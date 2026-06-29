# AngularDotnet2026

A **multi-product monorepo** for Angular + .NET product ideas, POCs, demos, and launch-ready code.

Each product lives in its own folder under `products/`. Nothing here overwrites or mixes with [angular15apps](https://github.com/amitjoshipune/angular15apps) (legacy Angular experiments) or other repos.

## Products

| Product | Folder | Stack | Status |
|---------|--------|-------|--------|
| India Stock Report Agent | [`products/india-stock-report-agent`](products/india-stock-report-agent) | .NET 8 Windows Service | Active |
| ShoppingBuddy | _planned_ `products/shopping-buddy` | Angular + .NET 8 + SQL | In angular15apps or separate branch |
| _Product 3–5_ | `products/<name>` | Per product | Pipeline |

## Quick start

```powershell
# India Stock Report Agent
cd products/india-stock-report-agent
dotnet run
```

## Repository layout

```
AngularDotnet2026/
├── docs/                          # Cross-product guides (branching, Azure, VC demos)
├── products/                      # One folder per product — never mix code between these
│   └── india-stock-report-agent/
│       ├── IndiaStockReportAgent.csproj
│       ├── Services/
│       └── README.md
├── templates/                     # Scaffolds for new Angular + .NET products
└── README.md
```

## Where things live (avoid confusion)

| Repo | Purpose |
|------|---------|
| **AngularDotnet2026** (this repo) | New products: Angular + .NET 6/8, AI, cloud POCs |
| **angular15apps** | Older Angular-only experiments; keep separate |
| **Per-product repo** (optional later) | Spin out when a product gets funding / team |

> **Do not merge** [angular15apps PR #1](https://github.com/amitjoshipune/angular15apps/pull/1) — that work now lives here under `products/india-stock-report-agent`.

## Docs

- [Engineering handbook](docs/ENGINEERING-HANDBOOK.md) — structure, branching, cost, Cursor, Azure, VC demos

## License

MIT per product unless noted otherwise.
