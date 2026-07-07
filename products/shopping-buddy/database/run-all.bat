@echo off
REM Run all ShoppingBuddy SQL scripts against SQL Server.
REM Default: DESKTOP-J5AN1SG\SQLEXPRESS (override with set SB_SQL_SERVER=your\instance)

if "%SB_SQL_SERVER%"=="" set SB_SQL_SERVER=DESKTOP-J5AN1SG\SQLEXPRESS
set SCRIPT_DIR=%~dp0

echo Target server: %SB_SQL_SERVER%
echo.

echo [1/5] Creating database ...
sqlcmd -S %SB_SQL_SERVER% -E -i "%SCRIPT_DIR%001_create_database.sql"
if errorlevel 1 goto :error

echo [2/5] Creating schema ...
sqlcmd -S %SB_SQL_SERVER% -E -i "%SCRIPT_DIR%002_schema.sql"
if errorlevel 1 goto :error

echo [3/5] Seeding data ...
sqlcmd -S %SB_SQL_SERVER% -E -i "%SCRIPT_DIR%003_seed_data.sql"
if errorlevel 1 goto :error

echo [4/5] Creating SQL auth logins (requires Mixed Mode — see README if this step fails) ...
sqlcmd -S %SB_SQL_SERVER% -E -i "%SCRIPT_DIR%004_sql_logins.sql"
if errorlevel 1 (
    echo.
    echo WARNING: SQL login step failed. Windows auth still works.
    echo Enable Mixed Mode and re-run 004_sql_logins.sql in SSMS.
)

echo [5/5] Booking workflow (UserRoles, PendingBuddy) ...
sqlcmd -S %SB_SQL_SERVER% -E -i "%SCRIPT_DIR%007_booking_workflow.sql"
echo.
echo Done. Database ShoppingBuddy is ready on %SB_SQL_SERVER%.
goto :eof

:error
echo.
echo FAILED. Check SQL Server Express is running and sqlcmd is on PATH.
exit /b 1
