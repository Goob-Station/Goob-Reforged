@echo off
cd ../../

call dotnet run --project Modules\GoobStation\Content.Goobstation.Client --no-build %*

pause
