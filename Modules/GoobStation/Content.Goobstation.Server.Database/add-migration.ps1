#!/usr/bin/env pwsh

param([String]$name)

if ($name -eq "")
{
    Write-Error "must specify migration name"
    exit
}

dotnet ef migrations add --context GoobStationSqliteServerDbContext -o Migrations/Sqlite $name
dotnet ef migrations add --context GoobStationPostgresServerDbContext -o Migrations/Postgres $name
