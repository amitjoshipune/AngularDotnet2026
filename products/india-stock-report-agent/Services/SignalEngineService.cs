using IndiaStockReportAgent.Configuration;
using IndiaStockReportAgent.Models;
using Microsoft.Extensions.Options;

namespace IndiaStockReportAgent.Services;

public interface ISignalEngineService
{
    Task<DailyReport> BuildDailyReportAsync(DateOnly reportDate, CancellationToken cancellationToken);
}

public sealed class SignalEngineService : ISignalEngineService
{
    private readonly ReportSettings _settings;
    private readonly INewsAggregatorService _newsService;
    private readonly IMarketDataService _marketDataService;
    private readonly ILogger<SignalEngineService> _logger;

    public SignalEngineService(
        IOptions<ReportSettings> settings,
        INewsAggregatorService newsService,
        IMarketDataService marketDataService,
        ILogger<SignalEngineService> logger)
    {
        _settings = settings.Value;
        _newsService = newsService;
        _marketDataService = marketDataService;
        _logger = logger;
    }

    public async Task<DailyReport> BuildDailyReportAsync(DateOnly reportDate, CancellationToken cancellationToken)
    {
        var news = await _newsService.FetchLatestNewsAsync(cancellationToken);
        var activeThemes = news
            .SelectMany(item => item.MatchedThemes)
            .GroupBy(theme => theme)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .Take(8)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var recommendations = new List<StockRecommendation>();

        foreach (var horizon in Enum.GetValues<InvestmentHorizon>())
        {
            var horizonEntries = _settings.Watchlist
                .Where(entry => entry.Horizons.Contains(horizon.ToString(), StringComparer.OrdinalIgnoreCase))
                .ToList();

            foreach (var entry in horizonEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var snapshot = await _marketDataService.GetPriceSnapshotAsync(entry.Symbol, cancellationToken);
                var recommendation = ScoreStock(entry, horizon, snapshot, news, activeThemes);
                recommendations.Add(recommendation);
            }
        }

        return new DailyReport
        {
            ReportDate = reportDate,
            MarketSummary = BuildMarketSummary(news, activeThemes),
            TopNews = news.Take(10).ToList(),
            Recommendations = recommendations
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Horizon)
                .ToList(),
            MacroRisks = BuildMacroRisks(news),
            MacroTailwinds = BuildMacroTailwinds(news),
            Budget = new BudgetSummary
            {
                MinBudgetInr = _settings.MinBudgetInr,
                MaxBudgetInr = _settings.MaxBudgetInr,
                AllocationNote = "With ₹10,000–25,000, prefer 1–2 positions per horizon. Avoid over-diversifying into 5+ stocks."
            }
        };
    }

    private StockRecommendation ScoreStock(
        WatchlistEntrySettings entry,
        InvestmentHorizon horizon,
        PriceSnapshot snapshot,
        IReadOnlyList<NewsItem> news,
        IReadOnlySet<string> activeThemes)
    {
        var score = 50;
        var reasons = new List<string>();
        var risks = new List<string>();

        var themeMatches = entry.ThemeTags
            .Where(tag => activeThemes.Contains(tag))
            .ToList();

        score += themeMatches.Count * 8;
        if (themeMatches.Count > 0)
        {
            reasons.Add($"News themes today: {string.Join(", ", themeMatches)}");
        }

        if (snapshot.HasData)
        {
            score += ScoreMomentum(snapshot, horizon, reasons, risks);
        }
        else
        {
            risks.Add("Live price data unavailable; signal based mainly on news themes.");
        }

        score += ScoreSectorContext(entry.Sector, news, reasons);

        var signal = score switch
        {
            >= 75 => TradeSignal.StrongBuy,
            >= 62 => TradeSignal.Buy,
            >= 48 => TradeSignal.Hold,
            >= 35 => TradeSignal.Sell,
            _ => TradeSignal.StrongSell
        };

        var quantity = 0;
        var estimatedCost = 0m;
        if (snapshot.HasData && snapshot.LastPrice > 0)
        {
            quantity = CalculateQuantity(snapshot.LastPrice, _settings.MaxBudgetInr, horizon);
            estimatedCost = quantity * snapshot.LastPrice;
        }

        return new StockRecommendation
        {
            Symbol = entry.Symbol,
            CompanyName = entry.CompanyName,
            Exchange = entry.Exchange,
            Horizon = horizon,
            Signal = signal,
            LastPrice = snapshot.HasData ? snapshot.LastPrice : null,
            ChangePercent = snapshot.HasData ? snapshot.ChangePercent : null,
            WeekChangePercent = snapshot.HasData ? snapshot.WeekChangePercent : null,
            MonthChangePercent = snapshot.HasData ? snapshot.MonthChangePercent : null,
            Score = score,
            SuggestedQuantity = quantity,
            EstimatedCostInr = decimal.Round(estimatedCost, 2),
            Reasons = reasons,
            Risks = risks
        };
    }

    private static int ScoreMomentum(
        PriceSnapshot snapshot,
        InvestmentHorizon horizon,
        List<string> reasons,
        List<string> risks)
    {
        var score = 0;
        var momentum = horizon switch
        {
            InvestmentHorizon.OneWeek => snapshot.WeekChangePercent,
            InvestmentHorizon.OneMonth => snapshot.MonthChangePercent,
            InvestmentHorizon.OneQuarter => snapshot.MonthChangePercent,
            InvestmentHorizon.OneYear => snapshot.MonthChangePercent,
            _ => snapshot.ChangePercent
        };

        if (momentum > 2)
        {
            score += 10;
            reasons.Add($"Positive {horizon} momentum: {momentum:0.##}%");
        }
        else if (momentum < -2)
        {
            score -= 8;
            risks.Add($"Negative {horizon} momentum: {momentum:0.##}%");
        }
        else
        {
            reasons.Add($"Momentum is neutral for {horizon}.");
        }

        if (snapshot.ChangePercent > 1.5m)
        {
            score += 4;
            reasons.Add($"Strong daily move: {snapshot.ChangePercent:0.##}%");
        }
        else if (snapshot.ChangePercent < -1.5m)
        {
            score -= 4;
            risks.Add($"Weak daily move: {snapshot.ChangePercent:0.##}%");
        }

        return score;
    }

    private static int ScoreSectorContext(string sector, IReadOnlyList<NewsItem> news, List<string> reasons)
    {
        var sectorNews = news.Count(item =>
            item.Title.Contains(sector, StringComparison.OrdinalIgnoreCase) ||
            item.MatchedThemes.Any(theme => theme.Contains(sector, StringComparison.OrdinalIgnoreCase)));

        if (sectorNews >= 2)
        {
            reasons.Add($"{sector} sector is in today's headlines.");
            return 6;
        }

        return 0;
    }

    private static int CalculateQuantity(decimal price, decimal maxBudget, InvestmentHorizon horizon)
    {
        var allocation = horizon switch
        {
            InvestmentHorizon.OneWeek => 0.35m,
            InvestmentHorizon.OneMonth => 0.45m,
            InvestmentHorizon.OneQuarter => 0.55m,
            InvestmentHorizon.OneYear => 0.65m,
            _ => 0.40m
        };

        var budgetSlice = maxBudget * allocation;
        var quantity = (int)Math.Floor(budgetSlice / price);
        return Math.Max(quantity, 0);
    }

    private static string BuildMarketSummary(IReadOnlyList<NewsItem> news, IReadOnlySet<string> activeThemes)
    {
        var headline = news.FirstOrDefault()?.Title ?? "No major headline captured from configured feeds.";
        var themes = activeThemes.Count == 0
            ? "No dominant theme detected."
            : $"Dominant themes: {string.Join(", ", activeThemes)}.";

        return $"{headline} {themes}";
    }

    private static IReadOnlyList<string> BuildMacroRisks(IReadOnlyList<NewsItem> news)
    {
        var risks = new List<string>();
        if (news.Any(item => item.MatchedThemes.Contains("monsoon")))
        {
            risks.Add("Deficient monsoon risk may hurt rural demand (FMCG, tractors, agri-linked stocks).");
        }

        if (news.Any(item => item.MatchedThemes.Contains("geopolitics")))
        {
            risks.Add("West Asia tensions remain an overhang for oil, aviation, and sentiment.");
        }

        if (news.Any(item => item.MatchedThemes.Contains("it")))
        {
            risks.Add("IT sector facing global demand and margin pressure despite long-term appeal.");
        }

        if (risks.Count == 0)
        {
            risks.Add("No major macro risk keyword detected in today's feeds; still monitor global cues.");
        }

        return risks;
    }

    private static IReadOnlyList<string> BuildMacroTailwinds(IReadOnlyList<NewsItem> news)
    {
        var tailwinds = new List<string>();
        if (news.Any(item => item.MatchedThemes.Contains("crude")))
        {
            tailwinds.Add("Cooling crude supports OMCs, autos, and margin recovery from Q2.");
        }

        if (news.Any(item => item.MatchedThemes.Contains("banking")))
        {
            tailwinds.Add("Banks benefiting from RBI measures and stable credit environment.");
        }

        if (news.Any(item => item.MatchedThemes.Contains("earnings")))
        {
            tailwinds.Add("Q1 FY27 results season may create stock-specific opportunities.");
        }

        if (news.Any(item => item.MatchedThemes.Contains("pharma")))
        {
            tailwinds.Add("Pharma showing relative strength amid broader market caution.");
        }

        if (tailwinds.Count == 0)
        {
            tailwinds.Add("Nifty holding above 24,000 support with moderating volatility.");
        }

        return tailwinds;
    }
}
