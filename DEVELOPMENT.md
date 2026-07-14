# EventHighway — Developer Guide

## Database Providers

EventHighway is provider-agnostic. `EventHighway.Core` ships no database engine; consumers
install exactly one provider package and pass its `IStorageBrokerProvider` to the client:

| Package | Provider class |
|---------|----------------|
| `EventHighway.SqlServer` | `SqlServerStorageBrokerProvider` |
| `EventHighway.PostgreSql` | `PostgreSqlStorageBrokerProvider` |

Each provider package carries its own EF Core migrations and registers itself via
`IStorageBrokerProvider.Configure` / `ConfigureModel`. Provider-specific schema concerns
(e.g. PostgreSQL `timestamptz(6)` precision and microsecond truncation) live in the provider's
`ConfigureModel` — never in Core.

## Prerequisites

- .NET 10 SDK
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — for running the databases
- `dotnet ef` global tool (only needed when generating migrations):

```powershell
dotnet tool install --global dotnet-ef
```

## Local databases

```powershell
docker compose up -d      # start SQL Server (localhost,1433) and PostgreSQL (localhost:5432)
docker compose ps         # wait until both report healthy
docker compose stop       # stop, keeping data
docker compose down -v    # wipe databases and start clean
```

Connection strings (matching `docker-compose.yml` and CI):

**SQL Server**
```
Server=localhost,1433;Database=EventHighwayDb;User Id=sa;Password=Your_password123!;TrustServerCertificate=True;MultipleActiveResultSets=true;Pooling=false
```

**PostgreSQL**
```
Host=localhost;Port=5432;Database=EventHighwayDb;Username=postgres;Password=postgres;Pooling=false
```

> On Windows the acceptance tests default to LocalDB (see
> `EventHighway.Core.Tests.Acceptance/appsettings.json`) so they also run without Docker.

### When LocalDB will not start

Symptom — the app dies on `Database.Migrate()` with:

```
SqlException: ... (provider: SQL Network Interfaces, error: 50 - Local Database Runtime error
occurred. Error occurred during LocalDB instance startup: SQL Server process failed to start.)
 ---> Win32Exception (0x89C5010A)
```

There are two causes, and they look identical from the app. Check them in this order:

**1. An orphaned engine process.** `sqllocaldb info MSSQLLocalDB` reports `State: Stopped`, yet a
`sqlservr.exe` for the instance is still alive — holding `master.mdf`, so every start attempt dies
instantly and writes *nothing* to the error log. `sqllocaldb stop -k` claims success but does not
reach it. Kill it by PID and start the instance:

```powershell
# which sqlservr belongs to which instance (leave the "ProjectModels" one alone — that is VS's)
Get-CimInstance Win32_Process -Filter "Name='sqlservr.exe'" | Select-Object ProcessId, CommandLine

Stop-Process -Id <the MSSQLLocalDB pid> -Force
sqllocaldb start MSSQLLocalDB
```

**2. `AUTO_CLOSE` back on.** LocalDB creates every user database with `AUTO_CLOSE ON` — the engine
shuts the database down whenever the last connection drops. A console app never notices; a web app
reconnecting all day turns it into exactly the startup failure above. **A database that is dropped
and recreated comes back with it on**, and setting it on `model` does *not* prevent this (verified
on the v17 instance: a fresh `EventHighwayDB` came back `1` while `model` was `0`).

```sql
SELECT name, is_auto_close_on FROM sys.databases;   -- want every row 0
ALTER DATABASE [EventHighwayDB] SET AUTO_CLOSE OFF;
```

`EventHighway.ClientV2.SubstrateApi` now does this for `EventHighwayDB` on startup
(`SubstrateApiRegistration.DisableAutoCloseOnSubstrateDatabase`), and it is a database-level
setting — so starting that sample once heals it for every app sharing the database. The Core client
deliberately does not, since it also ships against real SQL Server and Postgres where an
unconditional `ALTER DATABASE` could fail on permissions.

## Running acceptance tests

The acceptance test suite reads two settings — `PROVIDER` (`sqlserver` | `postgres`) and
`CONNECTION_STRING` — from `appsettings.json`, overridable by environment variables.

Run against one or both providers with the helper script:

```powershell
.\run-acceptance-tests.ps1                       # both providers
.\run-acceptance-tests.ps1 -Provider sqlserver
.\run-acceptance-tests.ps1 -Provider postgres
```

Or manually:

```powershell
$env:PROVIDER = "postgres"
$env:CONNECTION_STRING = "Host=localhost;Port=5432;Database=EventHighwayDb;Username=postgres;Password=postgres;Pooling=false"
dotnet test EventHighway.Core.Tests.Acceptance
```

## Generating migrations

Migrations live in each provider package — Core has none. Whenever the EF model changes
(new entity, new property, index, etc.), scaffold a migration **twice**, once per provider.
The design-time factories read `CONNECTION_STRING` from the environment and fall back to a
local default, so no code changes are needed:

```powershell
# SQL Server
dotnet ef migrations add <MigrationName> `
    --project EventHighway.SqlServer `
    --startup-project EventHighway.SqlServer `
    --context StorageBroker

# PostgreSQL
dotnet ef migrations add <MigrationName> `
    --project EventHighway.PostgreSql `
    --startup-project EventHighway.PostgreSql `
    --context StorageBroker
```

Preview the SQL before committing:

```powershell
dotnet ef migrations script `
    --project EventHighway.PostgreSql `
    --startup-project EventHighway.PostgreSql `
    --context StorageBroker
```

Migrations are applied automatically at runtime — the client calls `Database.Migrate()`
on startup using the migrations assembly the provider registered.

### Model-change checklist

- [ ] Update the entity class and its fluent configuration in `EventHighway.Core`
- [ ] `dotnet ef migrations add` for `EventHighway.SqlServer`
- [ ] `dotnet ef migrations add` for `EventHighway.PostgreSql`
- [ ] Verify both generated scripts (`dotnet ef migrations script`)
- [ ] `.\run-acceptance-tests.ps1` — both providers green before opening a PR

## Continuous integration

`.github/workflows/build.yml` is generated by `EventHighway.Infrastructure`
(`ScriptGenerationService`). **Never edit the workflow by hand** — change the generator and
regenerate, or the next generation run will overwrite your edit. The pipeline runs:

- **Build (Windows)** — full solution build + unit and acceptance tests on LocalDB
- **Build & Test (DB matrix)** — acceptance tests on Linux against real SQL Server and
  PostgreSQL service containers, one matrix leg per provider

## Adding a new provider

1. Create `EventHighway.<Provider>` referencing `EventHighway.Core`.
2. Implement `IStorageBrokerProvider` with the `Use*` call and `MigrationsAssembly`.
3. Add a design-time `IDesignTimeDbContextFactory<StorageBroker>`.
4. `dotnet ef migrations add InitialCreate` for the new project.
5. Add the provider to the `ClientBroker` switch in the acceptance tests.
6. Add a matrix entry in `ScriptGenerationService` and regenerate the workflow.

No changes to `EventHighway.Core` or existing providers are required.
