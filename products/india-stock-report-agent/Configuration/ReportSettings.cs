namespace IndiaStockReportAgent.Configuration;

public sealed class ReportSettings
{
    public const string SectionName = "ReportSettings";

    public decimal MinBudgetInr { get; set; } = 10000;
    public decimal MaxBudgetInr { get; set; } = 25000;
    public string ReportOutputDirectory { get; set; } = "Reports";
    public int ReportRetentionDays { get; set; } = 30;
    public string ReportTimeIst { get; set; } = "08:00";
    public bool RunImmediatelyOnStartup { get; set; } = true;
    public string TimeZoneId { get; set; } = "Asia/Kolkata";
    public string[] NewsRssFeeds { get; set; } =
    [
        "https://economictimes.indiatimes.com/markets/rssfeeds/1977021501.cms",
        "https://www.moneycontrol.com/rss/marketreports.xml",
        "https://www.livemint.com/rss/markets"
    ];
    public WatchlistEntrySettings[] Watchlist { get; set; } = DefaultWatchlist();
    public BrokerSettings Brokers { get; set; } = new();

    private static WatchlistEntrySettings[] DefaultWatchlist() =>
    [
        new() { Symbol = "DRREDDY", CompanyName = "Dr Reddy's Laboratories", Sector = "Pharma", Horizons = ["OneWeek", "OneMonth"], ThemeTags = ["pharma", "healthcare", "defensive"] },
        new() { Symbol = "MARUTI", CompanyName = "Maruti Suzuki", Sector = "Auto", Horizons = ["OneWeek", "OneMonth"], ThemeTags = ["auto", "crude", "consumer"] },
        new() { Symbol = "SBIN", CompanyName = "State Bank of India", Sector = "Banking", Horizons = ["OneWeek", "OneMonth", "OneQuarter"], ThemeTags = ["banking", "financials", "rbi"] },
        new() { Symbol = "JSWSTEEL", CompanyName = "JSW Steel", Sector = "Metals", Horizons = ["OneMonth", "OneQuarter"], ThemeTags = ["metals", "steel", "commodity"] },
        new() { Symbol = "ICICIBANK", CompanyName = "ICICI Bank", Sector = "Banking", Horizons = ["OneMonth", "OneQuarter", "OneYear"], ThemeTags = ["banking", "financials"] },
        new() { Symbol = "HDFCBANK", CompanyName = "HDFC Bank", Sector = "Banking", Horizons = ["OneMonth", "OneQuarter", "OneYear"], ThemeTags = ["banking", "financials"] },
        new() { Symbol = "ETERNAL", CompanyName = "Eternal (Zomato)", Sector = "Consumer Tech", Horizons = ["OneWeek", "OneMonth"], ThemeTags = ["consumer", "new-age", "urban"] },
        new() { Symbol = "NTPC", CompanyName = "NTPC", Sector = "Power", Horizons = ["OneQuarter", "OneYear"], ThemeTags = ["power", "utilities", "earnings"] },
        new() { Symbol = "HINDPETRO", CompanyName = "HPCL", Sector = "OMC", Horizons = ["OneMonth", "OneQuarter"], ThemeTags = ["omc", "crude", "energy"] },
        new() { Symbol = "TATASTEEL", CompanyName = "Tata Steel", Sector = "Metals", Horizons = ["OneQuarter", "OneYear"], ThemeTags = ["metals", "steel", "commodity"] },
        new() { Symbol = "BHARTIARTL", CompanyName = "Bharti Airtel", Sector = "Telecom", Horizons = ["OneQuarter", "OneYear"], ThemeTags = ["telecom", "defensive"] },
        new() { Symbol = "RELIANCE", CompanyName = "Reliance Industries", Sector = "Conglomerate", Horizons = ["OneYear"], ThemeTags = ["largecap", "conglomerate", "energy"] },
        new() { Symbol = "TCS", CompanyName = "Tata Consultancy Services", Sector = "IT", Horizons = ["OneYear"], ThemeTags = ["it", "largecap", "export"] },
        new() { Symbol = "INFY", CompanyName = "Infosys", Sector = "IT", Horizons = ["OneYear"], ThemeTags = ["it", "largecap", "export"] },
        new() { Symbol = "ITC", CompanyName = "ITC", Sector = "FMCG", Horizons = ["OneYear"], ThemeTags = ["fmcg", "defensive", "dividend"] },
        new() { Symbol = "SUNPHARMA", CompanyName = "Sun Pharmaceutical", Sector = "Pharma", Horizons = ["OneMonth", "OneQuarter"], ThemeTags = ["pharma", "healthcare", "defensive"] },
        new() { Symbol = "M&M", CompanyName = "Mahindra & Mahindra", Sector = "Auto", Horizons = ["OneMonth", "OneQuarter"], ThemeTags = ["auto", "rural", "suv"] },
        new() { Symbol = "TRENT", CompanyName = "Trent", Sector = "Retail", Horizons = ["OneWeek", "OneMonth"], ThemeTags = ["retail", "consumer", "urban"] }
    ];
}

public sealed class WatchlistEntrySettings
{
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Exchange { get; set; } = "NSE";
    public string Sector { get; set; } = string.Empty;
    public string[] Horizons { get; set; } = [];
    public string[] ThemeTags { get; set; } = [];
}

public sealed class BrokerSettings
{
    public AngelOneSettings AngelOne { get; set; } = new();
    public IciciDirectSettings IciciDirect { get; set; } = new();
}

public sealed class AngelOneSettings
{
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string TotpSecret { get; set; } = string.Empty;
}

public sealed class IciciDirectSettings
{
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string SessionToken { get; set; } = string.Empty;
}
