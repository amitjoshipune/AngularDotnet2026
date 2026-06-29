using IndiaStockReportAgent.Configuration;
using Microsoft.Extensions.Options;

namespace IndiaStockReportAgent.Services;

public interface IBrokerIntegrationService
{
    Task LogBrokerStatusAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Optional broker hooks for Angel One SmartAPI and ICICI Direct Breeze API.
/// This service only logs readiness today; wire credentials in appsettings to extend.
/// </summary>
public sealed class BrokerIntegrationService : IBrokerIntegrationService
{
    private readonly ReportSettings _settings;
    private readonly ILogger<BrokerIntegrationService> _logger;

    public BrokerIntegrationService(IOptions<ReportSettings> settings, ILogger<BrokerIntegrationService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task LogBrokerStatusAsync(CancellationToken cancellationToken)
    {
        if (_settings.Brokers.AngelOne.Enabled)
        {
            _logger.LogInformation(
                "Angel One SmartAPI enabled for client {ClientId}. Remember static IP registration is mandatory from Apr 2026.",
                _settings.Brokers.AngelOne.ClientId);
        }
        else
        {
            _logger.LogInformation("Angel One SmartAPI disabled. Enable in appsettings when ready.");
        }

        if (_settings.Brokers.IciciDirect.Enabled)
        {
            _logger.LogInformation("ICICI Direct Breeze API enabled. Session token must be refreshed daily.");
        }
        else
        {
            _logger.LogInformation("ICICI Direct Breeze API disabled. Enable in appsettings when ready.");
        }

        return Task.CompletedTask;
    }
}
