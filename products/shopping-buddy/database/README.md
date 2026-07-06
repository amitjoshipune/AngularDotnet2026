# ShoppingBuddy database (SQL Server)

Script-first setup — **no EF Core**. You run T-SQL; the .NET API uses **Dapper**.

## Prerequisites (Windows)

Install one of:

- **SQL Server LocalDB** (with Visual Studio / Build Tools), or
- **SQL Server Express** (`localhost\SQLEXPRESS`)

Ensure `sqlcmd` is on your PATH (SQL Server Command Line Tools).

## Quick setup (LocalDB)

From Git Bash or Command Prompt:

```bat
cd products\shopping-buddy\database
run-all.bat
```

Or run scripts manually in SSMS in order:

1. `001_create_database.sql`
2. `002_schema.sql`
3. `003_seed_data.sql`

## Connection strings

**LocalDB (default in `appsettings.json`):**

```
Server=(localdb)\MSSQLLocalDB;Database=ShoppingBuddy;Trusted_Connection=True;TrustServerCertificate=True;
```

**SQL Express:**

```
Server=localhost\SQLEXPRESS;Database=ShoppingBuddy;Trusted_Connection=True;TrustServerCertificate=True;
```

Update `backend/AuthApi/appsettings.Development.json` if you use Express.

## Demo logins (passwords stored as plain text — local MVP only)

| Email | Password | Role |
|-------|----------|------|
| customer@demo.com | Customer@123 | Customer |
| senior@demo.com | Senior@123 | Customer |
| meera@demo.com | Buddy@123 | Buddy |
| anjali@demo.com | Buddy@123 | Buddy |
| admin@example.com | Admin@123 | Admin |

## Verify

```sql
USE ShoppingBuddy;
SELECT COUNT(*) AS Users FROM dbo.Users;
SELECT COUNT(*) AS Buddies FROM dbo.Buddies;
SELECT * FROM dbo.Bookings;
```

## Re-run seed

`003_seed_data.sql` clears and re-inserts demo data. Safe for local dev.
