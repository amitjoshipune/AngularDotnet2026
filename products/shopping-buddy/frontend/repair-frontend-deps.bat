@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo.
echo === Repair corrupt rxjs / @angular packages (no full node_modules delete) ===
echo.

taskkill /F /IM node.exe >nul 2>&1

echo [1/4] Removing known-bad packages only...
if exist node_modules\rxjs rmdir /s /q node_modules\rxjs
if exist node_modules\@angular\common rmdir /s /q node_modules\@angular\common
if exist .angular rmdir /s /q .angular

echo [2/4] Reinstalling rxjs and @angular/common...
call npm install rxjs@7.8.1 @angular/common@15.2.10 --no-fund --no-audit
if errorlevel 1 exit /b 1

echo [3/4] Verifying...
call npm run verify:deps
if errorlevel 1 (
  echo.
  echo Repair not enough — run reinstall-frontend.bat for full clean install.
  exit /b 1
)

echo [4/4] Quick build test...
call npx ng build --configuration development
if errorlevel 1 exit /b 1

echo.
echo SUCCESS. Run: npm start
echo.
endlocal
