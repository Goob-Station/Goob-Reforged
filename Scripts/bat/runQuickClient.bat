@echo off
cd ../../

call dotnet run --project Goobstation.Bootstrap client --skip-build %*

pause
