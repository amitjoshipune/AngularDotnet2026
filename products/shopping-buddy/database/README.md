# ShoppingBuddy database (SQL Server)

Script-first setup — **no EF Core**. The .NET API uses **Dapper** behind repository **interfaces** (`IUserRepository`, `IBuddyRepository`, etc.) so you can swap to EF Core later without changing controllers.

## Your machine (Lenovo)

| Setting | Value |
|---------|--------|
| Server | `DESKTOP-J5AN1SG\SQLEXPRESS` |
| Edition | SQL Server Express 16.x |
| Windows login | `DESKTOP-J5AN1SG\LENOVO` |
| Auth (dev default) | Windows Authentication |

## Quick setup

```bat
cd products\shopping-buddy\database
run-all.bat
```

Scripts run in order: create DB → schema → seed → SQL logins.

## Connection strings (`appsettings.Development.json`)

**Windows auth (recommended for local dev):**

```
Server=DESKTOP-J5AN1SG\SQLEXPRESS;Database=ShoppingBuddy;Trusted_Connection=True;TrustServerCertificate=True;
```

**SQL auth — read/write (API / admin tools):**

```
Server=DESKTOP-J5AN1SG\SQLEXPRESS;Database=ShoppingBuddy;User Id=sb_app_rw;Password=SbFullControl_2026!;TrustServerCertificate=True;
```

**SQL auth — read-only (reports / BI / safe demos):**

```
Server=DESKTOP-J5AN1SG\SQLEXPRESS;Database=ShoppingBuddy;User Id=sb_app_ro;Password=SbReadOnly_2026!;TrustServerCertificate=True;
```

Change passwords before any shared or cloud environment.

## Mixed mode (Windows + SQL authentication)

SQL Express defaults to **Windows Authentication only**. To use `sb_app_ro` / `sb_app_rw`:

1. Open **SQL Server Configuration Manager**
2. **SQL Server Services** → **SQL Server (SQLEXPRESS)** → **Properties** → **Security**
3. Select **SQL Server and Windows Authentication mode**
4. **Restart** the SQLEXPRESS service
5. Run `004_sql_logins.sql` in SSMS (or re-run `run-all.bat`)

### Windows vs SQL users

| Type | Account | Use |
|------|---------|-----|
| Windows | `DESKTOP-J5AN1SG\LENOVO` | Your daily dev (`Trusted_Connection=True`) |
| SQL read-only | `sb_app_ro` | Reporting, safe read access |
| SQL full control | `sb_app_rw` | API when not using Windows auth, tools |

You do **not** need separate Windows readonly/full Windows accounts — use SQL logins for those roles. Mixed mode lets **both** Windows and SQL logins connect at the same time.

## Verify

```sql
USE ShoppingBuddy;
SELECT Email, Role FROM dbo.Users;
SELECT name, type_desc FROM sys.database_principals WHERE name LIKE 'sb_app%';
```

Test read-only login in SSMS: connect with **SQL Server Authentication** as `sb_app_ro` — `SELECT` works, `INSERT` fails.

## Override server name

```bat
set SB_SQL_SERVER=localhost\SQLEXPRESS
run-all.bat
```

## Re-run seed

`003_seed_data.sql` clears and re-inserts demo data. Safe for local dev.

## Repository interfaces (ORM swap)

```
IUserRepository      → UserRepository (Dapper)     → future: EfUserRepository
IBuddyRepository     → BuddyRepository (Dapper)    → future: EfBuddyRepository
IBookingRepository   → BookingRepository (Dapper)
ICatalogRepository   → CatalogRepository (Dapper)
```

SQL scripts remain the source of truth. EF Core later: `dotnet ef dbcontext scaffold` from this database.
