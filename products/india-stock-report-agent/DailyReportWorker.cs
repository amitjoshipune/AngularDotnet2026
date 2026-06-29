using IndiaStockReportAgent.Configuration;
using IndiaStockReportAgent.Services;
using Microsoft.Extensions.Options;

namespace IndiaStockReportAgent;

public sealed class DailyReportWorker : BackgroundService
{
    private readonly ReportSettings _settings;
    private readonly ISignalEngineService _signalEngine;
    private readonly IReportGeneratorService _reportGenerator;
    private readonly IBrokerIntegrationService _brokerIntegration;
    private readonly ILogger<DailyReportWorker> _logger;

    public DailyReportWorker(
        IOptions<ReportSettings> settings,
        ISignalEngineService signalEngine,
        IReportGeneratorService reportGenerator,
        IBrokerIntegrationService brokerIntegration,
        ILogger<DailyReportWorker> logger)
    {
        _settings = settings.Value;
        _signalEngine = signalEngine;
        _reportGenerator = reportGenerator;
        _brokerIntegration = brokerIntegration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _brokerIntegration.LogBrokerStatusAsync(stoppingToken);

        if (_settings.RunImmediatelyOnStartup)
        {
            await RunReportCycleAsync(stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun();
            _logger.LogInformation("Next report scheduled in {Delay}", delay);
            await Task.Delay(delay, stoppingToken);
            await RunReportCycleAsync(stoppingToken);
        }
    }

    private async Task RunReportCycleAsync(CancellationToken stoppingToken)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);
            var nowIst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone);
            var reportDate = DateOnly.FromDateTime(nowIst.DateTime);

            _logger.LogInformation("Generating daily stock report for {ReportDate}", reportDate);
            var report = await _signalEngine.BuildDailyReportAsync(reportDate, stoppingToken);
            var path = await _reportGenerator.GenerateAndSaveAsync(report, stoppingToken);
            _reportGenerator.CleanupOldReports();
            _logger.LogInformation("Daily report completed: {Path}", path);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Daily report generation failed");
        }
    }

    private TimeSpan GetDelayUntilNextRun()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);
        var nowIst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone);
        var scheduledTime = TimeOnly.Parse(_settings.ReportTimeIst);
        var nextRun = nowIst.Date.Add(scheduledTime.ToTimeSpan());

        if (nextRun <= nowIst)
        {
            nextRun = nextRun.AddDays(1);
        }

        return nextRun - nowIst;
    }
}
