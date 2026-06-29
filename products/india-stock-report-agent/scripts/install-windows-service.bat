@echo off
setlocal

echo Building India Stock Report Agent...
dotnet publish IndiaStockReportAgent\IndiaStockReportAgent.csproj -c Release -o publish

echo.
echo Installing Windows Service...
sc create "IndiaStockReportAgent" binPath= "%CD%\publish\IndiaStockReportAgent.exe" start= auto DisplayName= "India Stock Report Agent"
sc description "IndiaStockReportAgent" "Generates daily NSE/BSE stock research reports for short-term horizons."

echo.
echo Starting service...
sc start "IndiaStockReportAgent"

echo Done. Reports will be saved under the Reports folder.
pause
