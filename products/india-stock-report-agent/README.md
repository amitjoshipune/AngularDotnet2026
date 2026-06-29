# India Stock Report Agent

> **Location:** `products/india-stock-report-agent` in [AngularDotnet2026](https://github.com/amitjoshipune/AngularDotnet2026)

A .NET 8 **Windows Service** that generates a daily India stock research report for NSE/BSE watchlists across four horizons:

- **1 week** — catalyst / momentum plays
- **1 month** — sector + earnings-driven ideas
- **1 quarter** — thematic positions
- **1 year** — large-cap compounders

Built for a **₹10,000–25,000** budget and designed to run every trading morning before market open (default **08:00 IST**), retaining reports for **30 days**.

> **Disclaimer:** This tool produces informational research only. It is **not** SEBI-registered investment advice. Always verify signals and do your own due diligence before placing orders on ICICI Direct or Angel One.

---

## What it does (without heavy FA/TA)

Each morning the agent:

1. Pulls **Indian market news** from RSS feeds (Economic Times, Moneycontrol, Mint)
2. Fetches **end-of-day price momentum** from Yahoo Finance (`.NS` symbols)
3. Matches **news themes** (crude, monsoon, pharma, banking, earnings, etc.) to your watchlist
4. Scores each stock per horizon and outputs **Buy / Hold / Sell** style signals
5. Saves **HTML**, **Markdown**, and **JSON** reports under `Reports/`

No complex charting or DCF models — just news + light momentum scoring you can act on manually.

---

## Market context (29 Jun 2026)

Based on current headlines and earnings backdrop:

| Factor | Impact |
|--------|--------|
| Nifty ~24,056, 3-week gain | Cautious optimism |
| Brent crude ~$72 (cooling) | Tailwind for autos, OMCs, margins |
| Deficient monsoon risk | Headwind for rural/FMCG |
| Q1 FY27 results ahead | Stock-specific volatility |
| Pharma relative strength | Dr Reddy's, Sun Pharma in focus |
| Metals/OMCs strong in Q4 FY26 | JSW Steel, HPCL, Tata Steel |

### Thematic watchlist for your budget

These align with the default `appsettings.json` watchlist. With ₹10K–25K, stick to **1–2 stocks per horizon**.

| Horizon | Stocks to watch | Why (headline-level) |
|---------|-----------------|----------------------|
| **1 week** | DRREDDY, MARUTI, SBIN, TRENT | Pharma momentum, auto on lower crude, banking stability |
| **1 month** | ICICIBANK, ETERNAL, SUNPHARMA, M&M | Financials + consumer + pharma earnings season |
| **1 quarter** | JSWSTEEL, HINDPETRO, NTPC, BHARTIARTL | Metals/OMC earnings tailwind, power, telecom |
| **1 year** | RELIANCE, TCS, INFY, ITC | Large-cap quality; IT weak near-term but long-term compounders |

**Avoid over-diversifying:** at ₹10K you may only afford 1 share of several large caps. Prefer 1–2 concentrated positions.

---

## Quick start (Windows)

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 (for Windows Service hosting)
- Internet access for RSS and Yahoo Finance

### Run once (console)

```powershell
cd products/india-stock-report-agent
dotnet run
```

Reports appear in `products/india-stock-report-agent/Reports/` as:

- `stock-report-YYYY-MM-DD.html`
- `stock-report-YYYY-MM-DD.md`
- `stock-report-YYYY-MM-DD.json`

### Install as Windows Service

```powershell
cd products/india-stock-report-agent
dotnet publish -c Release -o publish
scripts\install-windows-service.bat
```

To remove:

```powershell
scripts\uninstall-windows-service.bat
```

---

## Configuration

Edit `appsettings.json`:

```json
{
  "ReportSettings": {
    "MinBudgetInr": 10000,
    "MaxBudgetInr": 25000,
    "ReportTimeIst": "08:00",
    "ReportRetentionDays": 30,
    "RunImmediatelyOnStartup": true
  }
}
```

### Customize watchlist

Add entries under `ReportSettings:Watchlist` with `Symbol`, `Horizons`, and `ThemeTags`. Horizons: `OneWeek`, `OneMonth`, `OneQuarter`, `OneYear`.

### Broker integration (optional, Phase 2)

Stubs exist for:

| Broker | API | Notes |
|--------|-----|-------|
| **Angel One** | [SmartAPI](https://smartapi.angelbroking.com/) | Static IP mandatory from Apr 2026 for algo orders |
| **ICICI Direct** | [Breeze API](https://api.icicidirect.com/) | Free API; session token refreshed daily |

Set `Enabled: true` and add credentials in `appsettings.json` or User Secrets. The agent currently **does not auto-place orders** — it only logs broker readiness. You execute manually on your demat apps.

---

## Daily workflow (suggested)

1. **08:00 IST** — Agent generates report
2. **08:30** — Read HTML/Markdown report
3. **09:00** — Compare signals with your existing holdings
4. **09:15+** — Place buy/sell on ICICI Direct or Angel One app
5. **Weekly** — Review 30-day report history in `Reports/`

### Signal guide

| Signal | Suggested action |
|--------|------------------|
| StrongBuy / Buy | Research further; consider adding if risk fits |
| Hold | No change unless news reverses |
| Sell / StrongSell | Review exit; don't panic-sell on one bad day |

---

## Project structure

```
IndiaStockReportAgent/
├── Configuration/ReportSettings.cs   # Budget, watchlist, RSS, brokers
├── Models/                           # Report DTOs
├── Services/
│   ├── NewsAggregatorService.cs      # RSS + theme detection
│   ├── MarketDataService.cs          # Yahoo Finance prices
│   ├── SignalEngineService.cs        # Scoring engine
│   ├── ReportGeneratorService.cs     # HTML/MD/JSON output
│   └── BrokerIntegrationService.cs   # Angel / ICICI hooks
├── DailyReportWorker.cs              # Scheduled worker
├── Program.cs                        # Host + Windows Service
└── scripts/                          # install/uninstall bat files
```

---

## Extending the agent

Future enhancements you can add:

- [ ] Angel One SmartAPI live LTP instead of Yahoo Finance
- [ ] ICICI Breeze portfolio sync (show your actual holdings in report)
- [ ] Telegram / email notification when report is ready
- [ ] Earnings calendar from NSE corporate announcements
- [ ] India VIX and FII/DII flow overlay

---

## License

MIT — use at your own risk. Not financial advice.
