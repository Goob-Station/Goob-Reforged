@echo off
cd ../../

call dotnet run --project Content.Modules.Server --no-build %*

pause
