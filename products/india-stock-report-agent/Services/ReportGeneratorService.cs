using System.Text;
using System.Text.Json;
using IndiaStockReportAgent.Configuration;
using IndiaStockReportAgent.Models;
using Microsoft.Extensions.Options;

namespace IndiaStockReportAgent.Services;

public interface IReportGeneratorService
{
    Task<string> GenerateAndSaveAsync(DailyReport report, CancellationToken cancellationToken);
    void CleanupOldReports();
}

public sealed class ReportGeneratorService : IReportGeneratorService
{
    private readonly ReportSettings _settings;
    private readonly ILogger<ReportGeneratorService> _logger;

    public ReportGeneratorService(IOptions<ReportSettings> settings, ILogger<ReportGeneratorService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GenerateAndSaveAsync(DailyReport report, CancellationToken cancellationToken)
    {
        var outputDirectory = Path.GetFullPath(_settings.ReportOutputDirectory);
        Directory.CreateDirectory(outputDirectory);

        var dateStamp = report.ReportDate.ToString("yyyy-MM-dd");
        var htmlPath = Path.Combine(outputDirectory, $"stock-report-{dateStamp}.html");
        var markdownPath = Path.Combine(outputDirectory, $"stock-report-{dateStamp}.md");
        var jsonPath = Path.Combine(outputDirectory, $"stock-report-{dateStamp}.json");

        var html = BuildHtml(report);
        var markdown = BuildMarkdown(report);
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(htmlPath, html, cancellationToken);
        await File.WriteAllTextAsync(markdownPath, markdown, cancellationToken);
        await File.WriteAllTextAsync(jsonPath, json, cancellationToken);

        _logger.LogInformation("Saved daily report to {HtmlPath}", htmlPath);
        return htmlPath;
    }

    public void CleanupOldReports()
    {
        var outputDirectory = Path.GetFullPath(_settings.ReportOutputDirectory);
        if (!Directory.Exists(outputDirectory))
        {
            return;
        }

        var cutoff = DateTime.UtcNow.Date.AddDays(-_settings.ReportRetentionDays);
        foreach (var file in Directory.EnumerateFiles(outputDirectory, "stock-report-*"))
        {
            var info = new FileInfo(file);
            if (info.CreationTimeUtc < cutoff)
            {
                info.Delete();
            }
        }
    }

    private static string BuildMarkdown(DailyReport report)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# India Stock Daily Report — {report.ReportDate:dd MMM yyyy}");
        builder.AppendLine();
        builder.AppendLine("> **Disclaimer:** Informational research only. Not SEBI-registered investment advice.");
        builder.AppendLine();
        builder.AppendLine("## Market Summary");
        builder.AppendLine(report.MarketSummary);
        builder.AppendLine();
        builder.AppendLine("## Macro Tailwinds");
        foreach (var item in report.MacroTailwinds)
        {
            builder.AppendLine($"- {item}");
        }

        builder.AppendLine();
        builder.AppendLine("## Macro Risks");
        foreach (var item in report.MacroRisks)
        {
            builder.AppendLine($"- {item}");
        }

        builder.AppendLine();
        builder.AppendLine($"## Budget Guidance (₹{report.Budget.MinBudgetInr:N0} – ₹{report.Budget.MaxBudgetInr:N0})");
        builder.AppendLine(report.Budget.AllocationNote);
        builder.AppendLine();

        foreach (var horizon in Enum.GetValues<InvestmentHorizon>())
        {
            builder.AppendLine($"## {FormatHorizon(horizon)} Picks");
            var picks = report.Recommendations
                .Where(item => item.Horizon == horizon)
                .OrderByDescending(item => item.Score)
                .Take(5)
                .ToList();

            if (picks.Count == 0)
            {
                builder.AppendLine("_No picks configured._");
                builder.AppendLine();
                continue;
            }

            foreach (var pick in picks)
            {
                builder.AppendLine($"### {pick.CompanyName} ({pick.Symbol}) — **{pick.Signal}** (Score: {pick.Score})");
                if (pick.LastPrice.HasValue)
                {
                    builder.AppendLine($"- Price: ₹{pick.LastPrice:0.##} | Day: {pick.ChangePercent:0.##}% | Week: {pick.WeekChangePercent:0.##}% | Month: {pick.MonthChangePercent:0.##}%");
                }

                if (pick.SuggestedQuantity > 0)
                {
                    builder.AppendLine($"- Suggested qty for your max budget: {pick.SuggestedQuantity} (~₹{pick.EstimatedCostInr:N0})");
                }

                foreach (var reason in pick.Reasons)
                {
                    builder.AppendLine($"- {reason}");
                }

                foreach (var risk in pick.Risks)
                {
                    builder.AppendLine($"- Risk: {risk}");
                }

                builder.AppendLine();
            }
        }

        builder.AppendLine("## Top Headlines");
        foreach (var item in report.TopNews)
        {
            builder.AppendLine($"- [{item.Title}]({item.Link}) — _{item.Source}_");
        }

        return builder.ToString();
    }

    private static string BuildHtml(DailyReport report)
    {
        var rows = report.Recommendations
            .OrderBy(item => item.Horizon)
            .ThenByDescending(item => item.Score)
            .Select(item => $@"
<tr>
  <td>{FormatHorizon(item.Horizon)}</td>
  <td>{item.Symbol}</td>
  <td>{item.CompanyName}</td>
  <td>{item.Signal}</td>
  <td>{item.Score}</td>
  <td>{(item.LastPrice.HasValue ? $"₹{item.LastPrice:0.##}" : "N/A")}</td>
  <td>{item.ChangePercent:0.##}%</td>
  <td>{item.SuggestedQuantity}</td>
  <td>₹{item.EstimatedCostInr:N0}</td>
</tr>");

        var newsItems = string.Join(string.Empty, report.TopNews.Select(item =>
            $"<li><a href=\"{item.Link}\">{Escape(item.Title)}</a> <em>({Escape(item.Source)})</em></li>"));

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"" />
  <title>India Stock Report — {report.ReportDate:dd MMM yyyy}</title>
  <style>
    body {{ font-family: Segoe UI, Arial, sans-serif; margin: 24px; color: #1f2937; }}
    h1, h2 {{ color: #111827; }}
    table {{ border-collapse: collapse; width: 100%; margin-top: 12px; }}
    th, td {{ border: 1px solid #d1d5db; padding: 8px; text-align: left; }}
    th {{ background: #f3f4f6; }}
    .note {{ background: #fff7ed; border-left: 4px solid #f59e0b; padding: 12px; }}
    .good {{ color: #047857; }}
    .risk {{ color: #b91c1c; }}
  </style>
</head>
<body>
  <h1>India Stock Daily Report — {report.ReportDate:dd MMM yyyy}</h1>
  <p class=""note""><strong>Disclaimer:</strong> Informational research only. Not SEBI-registered investment advice. Verify before placing orders on ICICI Direct or Angel One.</p>
  <h2>Market Summary</h2>
  <p>{Escape(report.MarketSummary)}</p>
  <h2>Macro Tailwinds</h2>
  <ul class=""good"">{string.Join(string.Empty, report.MacroTailwinds.Select(item => $"<li>{Escape(item)}</li>"))}</ul>
  <h2>Macro Risks</h2>
  <ul class=""risk"">{string.Join(string.Empty, report.MacroRisks.Select(item => $"<li>{Escape(item)}</li>"))}</ul>
  <h2>Budget Guidance</h2>
  <p>₹{report.Budget.MinBudgetInr:N0} – ₹{report.Budget.MaxBudgetInr:N0}. {Escape(report.Budget.AllocationNote)}</p>
  <h2>Recommendations</h2>
  <table>
    <thead>
      <tr>
        <th>Horizon</th><th>Symbol</th><th>Company</th><th>Signal</th><th>Score</th><th>Price</th><th>Day %</th><th>Qty</th><th>Est. Cost</th>
      </tr>
    </thead>
    <tbody>{string.Join(string.Empty, rows)}</tbody>
  </table>
  <h2>Top Headlines</h2>
  <ul>{newsItems}</ul>
</body>
</html>";
    }

    private static string FormatHorizon(InvestmentHorizon horizon) => horizon switch
    {
        InvestmentHorizon.OneWeek => "1 Week",
        InvestmentHorizon.OneMonth => "1 Month",
        InvestmentHorizon.OneQuarter => "1 Quarter",
        InvestmentHorizon.OneYear => "1 Year",
        _ => horizon.ToString()
    };

    private static string Escape(string value) =>
        value.Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal);
}
