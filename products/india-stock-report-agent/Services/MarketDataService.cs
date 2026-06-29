using System.Globalization;
using System.Text.Json;
using IndiaStockReportAgent.Models;

namespace IndiaStockReportAgent.Services;

public interface IMarketDataService
{
    Task<PriceSnapshot> GetPriceSnapshotAsync(string symbol, CancellationToken cancellationToken);
}

public sealed class MarketDataService : IMarketDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketDataService> _logger;

    public MarketDataService(HttpClient httpClient, ILogger<MarketDataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PriceSnapshot> GetPriceSnapshotAsync(string symbol, CancellationToken cancellationToken)
    {
        var yahooSymbol = $"{symbol}.NS";
        var url = $"https://query1.finance.yahoo.com/v8/finance/chart/{yahooSymbol}?interval=1d&range=3mo";

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var result = document.RootElement
                .GetProperty("chart")
                .GetProperty("result")[0];

            var meta = result.GetProperty("meta");
            var closes = result.GetProperty("indicators")
                .GetProperty("quote")[0]
                .GetProperty("close");

            var closeValues = closes.EnumerateArray()
                .Where(element => element.ValueKind == JsonValueKind.Number)
                .Select(element => element.GetDecimal())
                .ToList();

            if (closeValues.Count < 2)
            {
                return Empty(symbol);
            }

            var last = closeValues[^1];
            var previous = closeValues[^2];
            var weekBase = closeValues.Count >= 6 ? closeValues[^6] : closeValues[0];
            var monthBase = closeValues.Count >= 22 ? closeValues[^22] : closeValues[0];
            var changePercent = previous == 0 ? 0 : ((last - previous) / previous) * 100m;
            var weekChange = weekBase == 0 ? 0 : ((last - weekBase) / weekBase) * 100m;
            var monthChange = monthBase == 0 ? 0 : ((last - monthBase) / monthBase) * 100m;

            var volume = meta.TryGetProperty("regularMarketVolume", out var volumeElement)
                ? volumeElement.GetInt64()
                : 0;

            return new PriceSnapshot
            {
                Symbol = symbol,
                LastPrice = decimal.Round(last, 2),
                ChangePercent = decimal.Round(changePercent, 2),
                WeekChangePercent = decimal.Round(weekChange, 2),
                MonthChangePercent = decimal.Round(monthChange, 2),
                AverageVolume = volume,
                HasData = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch market data for {Symbol}", symbol);
            return Empty(symbol);
        }
    }

    private static PriceSnapshot Empty(string symbol) =>
        new()
        {
            Symbol = symbol,
            HasData = false
        };
}
