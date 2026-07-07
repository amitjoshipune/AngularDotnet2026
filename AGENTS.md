# AGENTS.md

Monorepo of Angular + .NET products under `products/`. See `README.md` and
`docs/ENGINEERING-HANDBOOK.md` for the product/branching overview.

## Cursor Cloud specific instructions

The cloud VM is **Ubuntu Linux** (no systemd, no Docker), while the repo docs assume
Windows + SQL Server Express. The environment setup adapts to Linux as follows.

### Toolchain (installed in the VM image, refreshed by the update script)
- **.NET 8 SDK** (`dotnet`), **Node 22 / npm** for Angular.
- **Microsoft SQL Server 2022** installed natively (package `mssql-server`) plus
  `sqlcmd` at `/opt/mssql-tools18/bin`.
- SQL Server 2022 is built for Ubuntu 22.04; on 24.04 its binary needs the
  `liblber-2.5.so.0` / `libldap-2.5.so.0` libraries, which are pre-copied into the VM
  image. This is a one-time image concern, not something to redo per session.

### Starting SQL Server (no systemd — run the binary directly)
```bash
sudo -u mssql ACCEPT_EULA=Y MSSQL_SA_PASSWORD='ShoppingBuddy_Dev2026!' MSSQL_PID=Developer \
  /opt/mssql/bin/sqlservr
```
Run it in the background (e.g. a tmux session) — it stays in the foreground otherwise.
It listens on `localhost:1433`. Data persists under `/var/opt/mssql`, so on later runs
the `ShoppingBuddy` database and seed data may already exist; the SQL scripts are
idempotent/re-seeding, so re-running them is safe.

Create + seed the ShoppingBuddy DB (from `products/shopping-buddy/database`):
```bash
export PATH="$PATH:/opt/mssql-tools18/bin"
for f in 001_create_database.sql 002_schema.sql 003_seed_data.sql 004_sql_logins.sql; do
  sqlcmd -S localhost,1433 -U sa -P 'ShoppingBuddy_Dev2026!' -C -b -i "$f"
done
```
`run-all.bat` is Windows-only (uses `-E` Windows auth); on Linux use the loop above with
SQL auth. `sqlcmd` here is v18, so it always needs `-C` (trust the self-signed cert).

### ShoppingBuddy backend API (.NET 8 + Dapper)
The committed connection strings point at a Windows SQLEXPRESS instance with Windows auth,
which does not exist on Linux. **Do not edit the config** — override the connection string
via environment variable (ASP.NET config binding) when running:
```bash
cd products/shopping-buddy/backend/AuthApi
export ConnectionStrings__ShoppingBuddy='Server=localhost,1433;Database=ShoppingBuddy;User Id=sa;Password=ShoppingBuddy_Dev2026!;TrustServerCertificate=True;'
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```
Serves on `http://localhost:5180` (Swagger at `/swagger`). Login/booking endpoints return
HTTP 503 with a "database unavailable" message if SQL Server is not running.

### ShoppingBuddy frontend (Angular 15)
```bash
cd products/shopping-buddy/frontend
npm start           # ng serve --proxy-config proxy.conf.json (proxies /api -> :5180)
```
NOTE (as of this branch): `ng serve` currently **fails to compile** due to two
pre-existing source bugs, not environment problems:
- `src/app/components/shopping-buddy/my-bookings/my-bookings.component.ts` imports
  `../../core/services/shopping-buddy.service` (should be `../../../core/...`).
- `src/app/core/data/mock-auth.data.ts` uses `role: 'Tester'`, which is not a member of
  the `UserRole` union (`'Customer' | 'Buddy' | 'Admin'`) in `core/models/auth.models.ts`.
Fix those in app code to run the UI. The backend API + SQL path works independently and is
the focus of this branch. Demo logins: `customer@demo.com` / `Customer@123`,
`admin@example.com` / `Admin@123`.

### India Stock Report Agent (.NET 8 worker, secondary product)
```bash
cd products/india-stock-report-agent
dotnet run          # RunImmediatelyOnStartup generates a report, then schedules the next
```
It fetches live RSS + Yahoo Finance data (needs internet) and writes HTML/MD/JSON into
`Reports/`. It is a long-running scheduler; stop it once a report is generated.

### Tests / lint
- Backend: `dotnet build` (no dedicated test project in the repo).
- Frontend: `npm test` runs Karma/Jasmine but needs a browser; there is no separate lint
  script defined in `package.json`.
