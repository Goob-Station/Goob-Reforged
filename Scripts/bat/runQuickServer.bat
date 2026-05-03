@echo off
cd ../../

call dotnet run --project Goobstation.Bootstrap server --skip-build %*

pause
