@echo off
cd ../../

call dotnet run --project Content.Modules.Client --no-build %*

pause
