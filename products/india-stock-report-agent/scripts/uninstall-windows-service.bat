@echo off
setlocal

echo Stopping and removing India Stock Report Agent service...
sc stop "IndiaStockReportAgent"
sc delete "IndiaStockReportAgent"
echo Done.
pause
