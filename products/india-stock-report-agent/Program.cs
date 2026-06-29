using IndiaStockReportAgent;
using IndiaStockReportAgent.Configuration;
using IndiaStockReportAgent.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "India Stock Report Agent";
});

builder.Services.Configure<ReportSettings>(builder.Configuration.GetSection(ReportSettings.SectionName));

builder.Services.AddHttpClient<INewsAggregatorService, NewsAggregatorService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("IndiaStockReportAgent/1.0");
});

builder.Services.AddHttpClient<IMarketDataService, MarketDataService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("IndiaStockReportAgent/1.0");
});

builder.Services.AddSingleton<ISignalEngineService, SignalEngineService>();
builder.Services.AddSingleton<IReportGeneratorService, ReportGeneratorService>();
builder.Services.AddSingleton<IBrokerIntegrationService, BrokerIntegrationService>();
builder.Services.AddHostedService<DailyReportWorker>();

var host = builder.Build();
host.Run();
