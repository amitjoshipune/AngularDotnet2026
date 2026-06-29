namespace IndiaStockReportAgent.Models;

public sealed class DailyReport
{
    public DateOnly ReportDate { get; init; }
    public string MarketSummary { get; init; } = string.Empty;
    public IReadOnlyList<NewsItem> TopNews { get; init; } = [];
    public IReadOnlyList<StockRecommendation> Recommendations { get; init; } = [];
    public IReadOnlyList<string> MacroRisks { get; init; } = [];
    public IReadOnlyList<string> MacroTailwinds { get; init; } = [];
    public BudgetSummary Budget { get; init; } = new();
}

public sealed class NewsItem
{
    public string Title { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string Link { get; init; } = string.Empty;
    public DateTimeOffset? PublishedAt { get; init; }
    public IReadOnlyList<string> MatchedThemes { get; init; } = [];
}

public sealed class StockRecommendation
{
    public string Symbol { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string Exchange { get; init; } = "NSE";
    public InvestmentHorizon Horizon { get; init; }
    public TradeSignal Signal { get; init; }
    public decimal? LastPrice { get; init; }
    public decimal? ChangePercent { get; init; }
    public decimal? WeekChangePercent { get; init; }
    public decimal? MonthChangePercent { get; init; }
    public int Score { get; init; }
    public int SuggestedQuantity { get; init; }
    public decimal EstimatedCostInr { get; init; }
    public IReadOnlyList<string> Reasons { get; init; } = [];
    public IReadOnlyList<string> Risks { get; init; } = [];
}

public sealed class BudgetSummary
{
    public decimal MinBudgetInr { get; init; }
    public decimal MaxBudgetInr { get; init; }
    public string AllocationNote { get; init; } = string.Empty;
}

public sealed class PriceSnapshot
{
    public string Symbol { get; init; } = string.Empty;
    public decimal LastPrice { get; init; }
    public decimal ChangePercent { get; init; }
    public decimal WeekChangePercent { get; init; }
    public decimal MonthChangePercent { get; init; }
    public long AverageVolume { get; init; }
    public bool HasData { get; init; }
}

public sealed class WatchlistEntry
{
    public string Symbol { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string Exchange { get; init; } = "NSE";
    public string Sector { get; init; } = string.Empty;
    public InvestmentHorizon[] Horizons { get; init; } = [];
    public string[] ThemeTags { get; init; } = [];
}
