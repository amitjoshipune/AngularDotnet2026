/*
  BookMyShoppingBuddy — diagnose and fix SQL auth logins (Error 233 / login failures)
  Run in SSMS connected as Windows Authentication (DESKTOP-J5AN1SG\LENOVO).
*/
USE master;
GO

PRINT '=== 1. Authentication mode ===';
SELECT
    CASE SERVERPROPERTY('IsIntegratedSecurityOnly')
        WHEN 1 THEN 'Windows only — SQL logins will FAIL until Mixed Mode is enabled'
        WHEN 0 THEN 'Mixed Mode OK — Windows + SQL logins allowed'
    END AS LoginMode;
GO

PRINT '=== 2. Server login status ===';
SELECT
    name,
    type_desc,
    is_disabled,
    default_database_name,
    create_date
FROM sys.server_principals
WHERE name IN (N'sb_app_ro', N'sb_app_rw');
GO

PRINT '=== 3. Enable logins, reset passwords, set default database ===';

IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'sb_app_ro')
BEGIN
    ALTER LOGIN sb_app_ro ENABLE;
    ALTER LOGIN sb_app_ro WITH
        PASSWORD = N'SbReadOnly_2026!',
        CHECK_POLICY = OFF,
        CHECK_EXPIRATION = OFF,
        DEFAULT_DATABASE = ShoppingBuddy;
    GRANT CONNECT SQL TO sb_app_ro;
END
GO

IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'sb_app_rw')
BEGIN
    ALTER LOGIN sb_app_rw ENABLE;
    ALTER LOGIN sb_app_rw WITH
        PASSWORD = N'SbFullControl_2026!',
        CHECK_POLICY = OFF,
        CHECK_EXPIRATION = OFF,
        DEFAULT_DATABASE = ShoppingBuddy;
    GRANT CONNECT SQL TO sb_app_rw;
END
GO

USE ShoppingBuddy;
GO

PRINT '=== 4. Database users and roles ===';

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'sb_app_ro')
    CREATE USER sb_app_ro FOR LOGIN sb_app_ro;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'sb_app_rw')
    CREATE USER sb_app_rw FOR LOGIN sb_app_rw;

ALTER ROLE db_datareader ADD MEMBER sb_app_ro;
ALTER ROLE db_owner ADD MEMBER sb_app_rw;
GO

SELECT
    dp.name AS UserName,
    r.name AS RoleName
FROM sys.database_role_members drm
INNER JOIN sys.database_principals dp ON dp.principal_id = drm.member_principal_id
INNER JOIN sys.database_principals r ON r.principal_id = drm.role_principal_id
WHERE dp.name IN (N'sb_app_ro', N'sb_app_rw')
ORDER BY dp.name, r.name;
GO

PRINT '=== Done. Test in SSMS: Authentication = SQL Server Authentication, Login = sb_app_rw ===';
GO
