@echo off
echo Pushing AngularDotnet2026 to GitHub...
cd /d "%~dp0"
git push -u origin cursor/india-stock-report-agent-0250
if %ERRORLEVEL% NEQ 0 (
  echo.
  echo Push failed. Run manually:
  echo   git remote set-url origin https://github.com/amitjoshipune/AngularDotnet2026.git
  echo   git push -u origin cursor/india-stock-report-agent-0250
)
pause
