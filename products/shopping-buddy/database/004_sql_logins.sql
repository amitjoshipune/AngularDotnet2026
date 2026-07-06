/*
  BookMyShoppingBuddy — SQL Server logins (SQL authentication)
  Prerequisite: enable Mixed Mode authentication and restart SQLEXPRESS (see database/README.md)

  Logins created:
    sb_app_ro  — read-only  (db_datareader)
    sb_app_rw  — read/write + DDL for demo (db_owner on ShoppingBuddy only)

  CHANGE PASSWORDS before any shared or cloud environment.
*/
USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'sb_app_ro')
BEGIN
    CREATE LOGIN sb_app_ro WITH PASSWORD = N'SbReadOnly_2026!', CHECK_POLICY = OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'sb_app_rw')
BEGIN
    CREATE LOGIN sb_app_rw WITH PASSWORD = N'SbFullControl_2026!', CHECK_POLICY = OFF;
END
GO

USE ShoppingBuddy;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'sb_app_ro')
BEGIN
    CREATE USER sb_app_ro FOR LOGIN sb_app_ro;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'sb_app_rw')
BEGIN
    CREATE USER sb_app_rw FOR LOGIN sb_app_rw;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    INNER JOIN sys.database_principals r ON r.principal_id = drm.role_principal_id
    INNER JOIN sys.database_principals m ON m.principal_id = drm.member_principal_id
    WHERE r.name = N'db_datareader' AND m.name = N'sb_app_ro'
)
BEGIN
    ALTER ROLE db_datareader ADD MEMBER sb_app_ro;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    INNER JOIN sys.database_principals r ON r.principal_id = drm.role_principal_id
    INNER JOIN sys.database_principals m ON m.principal_id = drm.member_principal_id
    WHERE r.name = N'db_owner' AND m.name = N'sb_app_rw'
)
BEGIN
    ALTER ROLE db_owner ADD MEMBER sb_app_rw;
END
GO

PRINT 'SQL logins ready: sb_app_ro (read-only), sb_app_rw (full control on ShoppingBuddy).';
GO
