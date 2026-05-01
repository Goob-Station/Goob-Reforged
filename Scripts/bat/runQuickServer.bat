@echo off
cd ../../

call dotnet run --project Modules\Goobstation\Content.Goobstation.Server --no-build %*

pause
