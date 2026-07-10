# Runs the acceptance test suite against one or both database providers.
# Requires the docker-compose databases to be running: docker compose up -d
#
# Usage:
#   .\run-acceptance-tests.ps1                       # both providers
#   .\run-acceptance-tests.ps1 -Provider sqlserver
#   .\run-acceptance-tests.ps1 -Provider postgres
param(
    [ValidateSet("both", "sqlserver", "postgres")]
    [string]$Provider = "both"
)

$SqlServerConnection =
    "Server=localhost,1433;Database=EventHighwayDb;User Id=sa;" +
    "Password=Your_password123!;TrustServerCertificate=True;" +
    "MultipleActiveResultSets=true;Pooling=false"

$PostgresConnection =
    "Host=localhost;Port=5432;Database=EventHighwayDb;" +
    "Username=postgres;Password=postgres;Pooling=false"

$exitCode = 0

if ($Provider -eq "sqlserver" -or $Provider -eq "both") {
    Write-Host "`n=== Acceptance tests: SQL Server ===`n" -ForegroundColor Cyan
    $env:PROVIDER = "sqlserver"
    $env:CONNECTION_STRING = $SqlServerConnection
    dotnet test EventHighway.Core.Tests.Acceptance --logger "console;verbosity=normal"

    if ($LASTEXITCODE -ne 0) { $exitCode = $LASTEXITCODE }
}

if ($Provider -eq "postgres" -or $Provider -eq "both") {
    Write-Host "`n=== Acceptance tests: PostgreSQL ===`n" -ForegroundColor Cyan
    $env:PROVIDER = "postgres"
    $env:CONNECTION_STRING = $PostgresConnection
    dotnet test EventHighway.Core.Tests.Acceptance --logger "console;verbosity=normal"

    if ($LASTEXITCODE -ne 0) { $exitCode = $LASTEXITCODE }
}

exit $exitCode
