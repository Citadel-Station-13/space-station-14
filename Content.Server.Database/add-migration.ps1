#!/usr/bin/env pwsh

param([String]$name)

if ($name -eq "")
{
    Write-Error "must specify migration name"
    exit
}

dotnet ef migrations add --context SqliteServerDbContext -o Migrations/Sqlite $name
dotnet ef migrations add --context PostgresServerDbContext -o Migrations/Postgres $name
dotnet ef migrations add --context CitadelSqliteContext -o Migrations/_Citadel/Sqlite $name
dotnet ef migrations add --context CitadelPostgresContext -o Migrations/_Citadel/Postgres $name
