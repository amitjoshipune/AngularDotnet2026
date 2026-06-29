# Engineering Handbook — AngularDotnet2026

A practical guide for running **5+ products** (Angular + .NET + DB + optional AI/cloud) as a solo founder using Cursor, GitHub, VS Code, Visual Studio, and Azure — without spaghetti code or runaway cost.

---

## 1. Repository strategy (recommended)

### Option A — Monorepo per “studio” (recommended for you now)

**One repo:** `AngularDotnet2026`  
**Rule:** Each product is an isolated folder under `products/<product-slug>/`.

```
products/
  shopping-buddy/
    frontend/          # Angular app
    backend/           # .NET 8 Web API
    database/          # SQL scripts, EF migrations
    docs/              # pitch deck notes, demo script
    README.md
  india-stock-report-agent/
    ...                # already a single .NET worker
  product-3/
    frontend/
    backend/
```

**Pros:** One place for Cursor context; shared templates; easy demos; cheap (one GitHub repo).  
**Cons:** Repo grows — mitigated by strict folder boundaries and `.cursor/rules` per product.

### Option B — One repo per product (when to split)

Split when **any** of these is true:

- Product has paying customers or a dedicated team
- CI/CD deploy pipelines conflict
- You raise seed funding and need clean cap-table / IP boundaries
- Repo clone time hurts daily work

Until then, **stay in the monorepo**.

### What NOT to do

| Avoid | Do instead |
|-------|------------|
| Dumping all apps in `angular15apps` | Use `AngularDotnet2026` for new work |
| Shared “utils” folder used by everyone | `templates/` for copy-paste scaffolds only |
| One `main` branch with half-finished products | Feature branches + product prefixes |
| Merging unrelated PRs | Close stale PRs; one PR = one product/feature |

---

## 2. Standard product folder template

When starting product #2 (e.g. ShoppingBuddy):

```
products/shopping-buddy/
├── README.md                 # What it is, how to run, demo URL
├── docs/
│   ├── PITCH.md              # 1-pager for VCs
│   └── DEMO-SCRIPT.md        # 3-minute demo flow
├── frontend/                 # ng new shopping-buddy
├── backend/                  # dotnet new webapi
├── database/
│   └── migrations/
├── infra/                    # optional: Bicep/Terraform for Azure
└── .cursor/
    └── rules                 # Cursor rules scoped to this product only
```

Copy from `templates/product-scaffold/` when ready.

---

## 3. Branching strategy

### Branch naming

```
product/<slug>/feature/<short-description>
product/<slug>/fix/<short-description>
product/<slug>/release/<version>
```

Examples:

- `product/shopping-buddy/feature/cart-checkout`
- `product/india-stock-report-agent/feature/telegram-alerts`
- `product/shopping-buddy/release/0.1.0`

### Flow

```
main (always deployable demos; tagged releases)
  └── product/<slug>/feature/...  → PR → main
```

**Rules:**

1. `main` should build — broken demos kill VC meetings.
2. One PR = one product (or one feature within one product).
3. Tag releases: `shopping-buddy-v0.1.0`, `stock-agent-v1.0.0`.
4. Use **GitHub Issues** with labels: `product:shopping-buddy`, `product:stock-agent`.

### Protect `main`

- Require PR (even if solo — future you will thank you)
- Optional: require `dotnet build` / `ng build` in GitHub Actions per changed product only

---

## 4. Cost-effective tooling stack

### Free / low-cost tier (bootstrap phase)

| Tool | Use | Cost |
|------|-----|------|
| **GitHub** | Code, Issues, Projects, Actions | Free (public/private) |
| **Cursor** | AI coding (you) | Your subscription |
| **VS Code** | Angular, scripts, light .NET | Free |
| **Visual Studio Community** | Heavy .NET, Windows Service debug | Free |
| **Azure free tier** | App Service F1, SQL basic trial, Static Web Apps | ~$0–25/mo if careful |
| **Vercel / Netlify** | Angular demo hosting | Free tier |
| **Supabase / Neon** | Postgres if not SQL Server | Free tier |

### Azure cost discipline (solo founder)

1. **One resource group per product:** `rg-shopping-buddy-dev`, `rg-stock-agent-dev`
2. **Shut down dev** App Service / SQL when not demoing (Azure Automation or manual stop)
3. **Static Web Apps** for Angular demos — cheaper than always-on App Service
4. **SQL Server:** start with **Azure SQL Serverless** or local SQL + Docker; move up when revenue exists
5. **AI:** Azure OpenAI only when demo needs it; use Cursor for dev-time AI instead

### Estimated monthly burn (5 products, dev only)

| Scenario | ~Monthly |
|----------|----------|
| GitHub + Cursor + local dev | $20–40 (Cursor only) |
| + 2 Azure demos (stopped nights) | $30–80 |
| + SQL + OpenAI for one product | $100–200 |

Keep prod empty until first paying user or VC term sheet.

---

## 5. Cursor + GitHub + IDE workflow

### Daily loop

1. Open **VS Code or Cursor** at repo root: `AngularDotnet2026`
2. Tell Cursor: *"Work only in `products/shopping-buddy`"* (scope matters)
3. Branch: `product/shopping-buddy/feature/...`
4. Commit small; push; open PR
5. Merge to `main`; tag if demo-ready

### Cursor rules per product

Add `products/<slug>/.cursor/rules`:

```
Only edit files under products/shopping-buddy/.
Stack: Angular 17+, .NET 8, EF Core, SQL Server.
Do not modify other products folders.
```

### Visual Studio

Open **solution per product**: `products/shopping-buddy/backend/ShoppingBuddy.sln`  
Do not create one giant solution with all 5 products — that creates spaghetti.

### GitHub Projects (free kanban)

Columns: **Ideation → POC → Demo → Pilot → Launch**  
One card per product; link Issues and PRs.

---

## 6. VC / sales / marketing without a team

You focus on: **code, design, ideation, PPTs, demo, promote**.

| Stage | Artifact | Location |
|-------|----------|----------|
| Ideation | Problem statement, 1-pager | `products/<slug>/docs/PITCH.md` |
| POC | Runnable `main` branch | This repo |
| Demo | 3-min script + hosted URL | `docs/DEMO-SCRIPT.md` + Azure Static Web App |
| Pitch | 10-slide deck | Store in `docs/decks/` (PDF) or Google Drive; link in README |
| Traction | Waitlist, LinkedIn posts | Link in product README |

**Demo URL pattern:** `https://shopping-buddy-demo.azurewebsites.net` (one per product)

---

## 7. Database strategy across products

| Product type | Suggested DB | Why |
|--------------|--------------|-----|
| CRUD app (ShoppingBuddy) | SQL Server / Azure SQL | Familiar, EF Core |
| Analytics / reports | SQL + read replicas later | |
| Flexible schema POC | Postgres (Neon) | Cheaper dev |
| AI / vectors | Azure AI Search or pgvector | Add only when needed |

**Rule:** One database **per product** in Azure — never share schemas between products.

---

## 8. AI integration (optional, per product)

Add AI only when the demo **requires** it:

| Use case | Approach |
|----------|----------|
| Dev assistance | Cursor (already have) |
| User-facing chat | Azure OpenAI + .NET SDK behind API |
| Stock/news agent | RSS + rules first (like stock agent); LLM summary later |

Keep API keys in **Azure Key Vault** or GitHub Secrets — never in repo.

---

## 9. Migration checklist: angular15apps → here

- [x] India Stock Report Agent → `products/india-stock-report-agent`
- [ ] ShoppingBuddy → `products/shopping-buddy` (copy from angular15apps when ready)
- [ ] Close [angular15apps PR #1](https://github.com/amitjoshipune/angular15apps/pull/1) without merging
- [ ] Archive or rename angular15apps README: "Legacy — see AngularDotnet2026"

---

## 10. Decision summary

| Question | Answer |
|----------|--------|
| One repo or five? | **One monorepo** until funding/team |
| angular15apps vs AngularDotnet2026? | **New work → AngularDotnet2026** |
| Will PR #1 mess up ShoppingBuddy? | **No**, if you don't merge it; use this repo instead |
| Branching? | `product/<slug>/feature/...` |
| Biggest cost saver? | Stop Azure resources when not demoing; local SQL for dev |

---

*Last updated: June 2026*
