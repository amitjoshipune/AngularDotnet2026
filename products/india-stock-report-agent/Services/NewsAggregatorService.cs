using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;
using IndiaStockReportAgent.Configuration;
using IndiaStockReportAgent.Models;
using Microsoft.Extensions.Options;

namespace IndiaStockReportAgent.Services;

public interface INewsAggregatorService
{
    Task<IReadOnlyList<NewsItem>> FetchLatestNewsAsync(CancellationToken cancellationToken);
}

public sealed class NewsAggregatorService : INewsAggregatorService
{
    private static readonly Dictionary<string, string[]> ThemeKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["crude"] = ["crude", "oil", "brent", "petroleum", "omc", "fuel"],
        ["monsoon"] = ["monsoon", "rainfall", "el nino", "kharif", "rural", "agri"],
        ["pharma"] = ["pharma", "drug", "laborator", "healthcare", "fda"],
        ["auto"] = ["auto", "maruti", "vehicle", "suv", "ev"],
        ["banking"] = ["bank", "rbi", "credit", "npa", "lending"],
        ["metals"] = ["metal", "steel", "aluminium", "copper", "commodity"],
        ["it"] = ["it sector", "infosys", "tcs", "tech mahindra", "genai", "software"],
        ["earnings"] = ["results", "earnings", "q1", "q2", "profit", "margin", "guidance"],
        ["geopolitics"] = ["iran", "west asia", "middle east", "hormuz", "war", "conflict"],
        ["fii"] = ["fii", "fpi", "foreign investor", "flows"],
        ["defensive"] = ["defensive", "fmcg", "dividend", "stable"],
        ["consumer"] = ["consumer", "retail", "urban demand", "discretionary"]
    };

    private readonly HttpClient _httpClient;
    private readonly ReportSettings _settings;
    private readonly ILogger<NewsAggregatorService> _logger;

    public NewsAggregatorService(
        HttpClient httpClient,
        IOptions<ReportSettings> settings,
        ILogger<NewsAggregatorService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NewsItem>> FetchLatestNewsAsync(CancellationToken cancellationToken)
    {
        var items = new List<NewsItem>();

        foreach (var feedUrl in _settings.NewsRssFeeds)
        {
            try
            {
                using var response = await _httpClient.GetAsync(feedUrl, cancellationToken);
                response.EnsureSuccessStatusCode();
                var xml = await response.Content.ReadAsStringAsync(cancellationToken);
                items.AddRange(ParseRss(xml, feedUrl));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch RSS feed {FeedUrl}", feedUrl);
            }
        }

        return items
            .OrderByDescending(item => item.PublishedAt ?? DateTimeOffset.MinValue)
            .DistinctBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
            .Take(25)
            .ToList();
    }

    private static IEnumerable<NewsItem> ParseRss(string xml, string feedUrl)
    {
        var document = XDocument.Parse(xml);
        var source = new Uri(feedUrl).Host.Replace("www.", string.Empty, StringComparison.OrdinalIgnoreCase);

        foreach (var item in document.Descendants("item"))
        {
            var title = item.Element("title")?.Value?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                continue;
            }

            var description = item.Element("description")?.Value ?? string.Empty;
            var combined = $"{title} {description}";
            var themes = DetectThemes(combined);

            yield return new NewsItem
            {
                Title = title,
                Source = source,
                Link = item.Element("link")?.Value ?? string.Empty,
                PublishedAt = ParseDate(item.Element("pubDate")?.Value),
                MatchedThemes = themes
            };
        }
    }

    internal static IReadOnlyList<string> DetectThemes(string text)
    {
        return ThemeKeywords
            .Where(pair => pair.Value.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .Select(pair => pair.Key)
            .ToList();
    }

    private static DateTimeOffset? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
        {
            return parsed;
        }

        return null;
    }
}
