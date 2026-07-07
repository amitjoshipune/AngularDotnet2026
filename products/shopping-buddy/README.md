# BookMyShoppingBuddy

**Verified shopping companions for women in Pune — portfolio demo (Angular 15 + .NET 8).**

Migrated from [angular15apps `master`](https://github.com/amitjoshipune/angular15apps/tree/master) into this monorepo under `products/shopping-buddy/`.

## What this demo shows

| Area | Implementation |
|------|----------------|
| Product concept | Women-only shopping buddies, public venues only, Pune localities |
| Safety & legal | `/shopping/safety` — rules, verification steps, Indian law checklist |
| Senior-friendly UI | Large controls, clear labels, simple 3-step flow |
| Angular 15 | Components, services, reactive forms, routing, guards |
| Auth | JWT login with .NET 8 API (`backend/AuthApi`) |
| API integration | CRUD items demo at `/items` |
| Documentation | `docs/product-vision.md`, `docs/legal-considerations.md`, `docs/faq.md`, `docs/roadmap.md` |

## Quick start

### 1. Database (one-time)

```bat
cd products\shopping-buddy\database
run-all.bat
```

See [`database/README.md`](database/README.md) for SQL Express connection strings.

### 2. Backend API

```bash
cd products/shopping-buddy/backend/AuthApi
dotnet run
```

Swagger: http://localhost:5180/swagger

### 3. Frontend

```bash
cd products/shopping-buddy/frontend
npm install
ng serve --port 4300
```

Open http://localhost:4300/login

**Demo logins:** `customer@demo.com` / `Customer@123` · `meera@demo.com` / `Buddy@123` (buddy + customer) · `admin@example.com` / `Admin@123`

See [`docs/mvp-roles-and-booking.md`](docs/mvp-roles-and-booking.md) for roles and booking workflow.

The Angular dev server proxies `/api` to `http://localhost:5180` via `frontend/proxy.conf.json`.

## Demo flow

1. **Sign in** — demo accounts on login page
2. **Home** — `/shopping` — product overview and trust badges
3. **Find buddy** — filter by Pune area (Wakad, Aundh, Chinchwad, …) and public mall
4. **Book** — accept safety rules, pick date/slot; status starts as **PendingBuddy** (buddy must confirm)
5. **Buddy requests** — log in as `meera@demo.com` → confirm or reject with reason
6. **My bookings** — customer sees status updates (Confirmed / Rejected)
7. **Safety & legal** — `/shopping/safety` (public, no login required)

## Project structure

```
products/shopping-buddy/
├── frontend/          # Angular 15 app (src/, angular.json, package.json)
├── backend/
│   └── AuthApi/       # ASP.NET Core 8 Web API — auth + items
├── database/
│   └── migrations/    # SQL scripts (placeholder)
└── docs/              # Vision, legal, FAQ, roadmap
```

## Legal disclaimer

This repository is **not legal advice**. Before any commercial launch, consult an Indian advocate for IT Act, DPDP Act 2023, intermediary guidelines, and app store policies. See `docs/legal-considerations.md`.

## Tech stack

- **Frontend:** Angular 15, Bootstrap 5, TypeScript, RxJS
- **Backend:** ASP.NET Core 8 Web API, JWT
- **Future:** SQL Server + EF Core, Capacitor → Android

## Portfolio pitch (one line)

*Built a safety-first Angular POC for verified women shopping companions in Pune — Aadhaar verification, public-venue-only meetings, senior-friendly booking flow, and documented compliance approach.*
