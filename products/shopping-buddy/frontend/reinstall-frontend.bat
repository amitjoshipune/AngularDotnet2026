@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo.
echo === BookMyShoppingBuddy — clean frontend reinstall ===
echo.

echo [1/5] Stopping Node processes (ng serve, etc.)...
taskkill /F /IM node.exe >nul 2>&1

echo [2/5] Removing old install folders...
if exist node_modules (
  ren node_modules node_modules_broken_%RANDOM% 2>nul
  if exist node_modules (
    echo Could not rename node_modules — close VS Code terminals and retry.
    exit /b 1
  )
)
if exist .angular rmdir /s /q .angular
if exist dist rmdir /s /q dist

echo [3/5] npm install (uses package-lock.json when present)...
call npm ci
if errorlevel 1 (
  echo npm ci failed — trying npm install...
  call npm install
  if errorlevel 1 exit /b 1
)

echo [4/5] Verifying @angular/common/http and rxjs...
if not exist "node_modules\@angular\common\fesm2020\http.mjs" (
  echo.
  echo ERROR: http.mjs still missing. Delete node_modules_broken_* folders and run this script again.
  exit /b 1
)
call npm run verify:deps
if errorlevel 1 exit /b 1

echo [5/5] Test build...
call npx ng build --configuration development
if errorlevel 1 (
  echo Build failed — see errors above.
  exit /b 1
)

echo.
echo SUCCESS. Start the app with:  npm start
echo Open: http://localhost:4300/login
echo.
endlocal
