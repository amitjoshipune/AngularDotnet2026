@echo off
REM Run all ShoppingBuddy SQL scripts against LocalDB (default).
REM Requires sqlcmd (SQL Server tools). Adjust -S for SQL Express: localhost\SQLEXPRESS

set SERVER=(localdb)\MSSQLLocalDB
set SCRIPT_DIR=%~dp0

echo Creating database on %SERVER% ...
sqlcmd -S %SERVER% -i "%SCRIPT_DIR%001_create_database.sql"
if errorlevel 1 goto :error

echo Creating schema ...
sqlcmd -S %SERVER% -i "%SCRIPT_DIR%002_schema.sql"
if errorlevel 1 goto :error

echo Seeding data ...
sqlcmd -S %SERVER% -i "%SCRIPT_DIR%003_seed_data.sql"
if errorlevel 1 goto :error

echo.
echo Done. Database ShoppingBuddy is ready.
goto :eof

:error
echo.
echo FAILED. Install SQL Server LocalDB or SQL Express and ensure sqlcmd is on PATH.
exit /b 1
