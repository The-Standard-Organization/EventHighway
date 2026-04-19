# Installing EventHighway

> [!TIP]
> ![V1](https://img.shields.io/badge/V1_(v2.10+)-Recommended-brightgreen?style=for-the-badge)
> This guide covers installation for the **V1** API surface.

---

## NuGet Package

EventHighway is distributed as a single NuGet package:

| Package | NuGet |
|---|---|
| **EventHighway** | [![Nuget](https://img.shields.io/nuget/v/EventHighway?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway) |

---

## Supported Platforms

EventHighway targets **.NET 10** and can be used in any application host that supports it:

| Platform | Supported | Notes |
|---|---|---|
| **Console Application** | ✅ | Simplest host — ideal for background workers, schedulers, and CLI tools |
| **ASP.NET Core Web API** | ✅ | Embed event publishing inside your API controllers or services |
| **Worker Service** | ✅ | Long-running hosted services for scheduled or continuous event processing |
| **Blazor Server** | ✅ | Publish events from server-side Blazor components |
| **Azure Functions** | ✅ | Use in isolated-process .NET 10 functions |
| **MAUI / WinForms / WPF** | ✅ | Desktop apps that need to push events to a central bus |
| **Unit / Integration Tests** | ✅ | Wire up `EventHighwayClient` in acceptance tests against a real or test database |

> EventHighway is platform-agnostic by design. Anywhere you can reference a .NET 10 class library and connect to SQL Server, you can use EventHighway.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- SQL Server instance (LocalDB, SQL Server Express, Azure SQL, or full SQL Server)

---

## Installation Methods

### .NET CLI

```bash
dotnet add package EventHighway
```

### Package Manager Console (Visual Studio)

```powershell
Install-Package EventHighway
```

### PackageReference (manual edit)

Add directly to your `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="EventHighway" Version="*" />
</ItemGroup>
```

> Replace `*` with the specific version you want to pin (e.g., `2.10.0`).

---

## Initializing the Client

`EventHighwayClient` is the single entry point. It accepts a SQL Server connection string, auto-runs EF Core migrations on first use, and exposes all V1 sub-clients:

```csharp
using EventHighway.Core.Clients.EventHighways;

var eventHighway = new EventHighwayClient(
    "Server=.;Database=EventHighwayDB;Trusted_Connection=True;TrustServerCertificate=True;");
```

On construction, `EventHighwayClient`:

1. Builds an internal `IServiceProvider` with all required services registered.
2. Runs `Database.Migrate()` to ensure the schema is up to date.
3. Exposes the following V1 client properties:

| Property | Type | Purpose |
|---|---|---|
| `EventAddressV1s` | `IEventAddressesV1Client` | Register and manage event addresses |
| `EventListenerV1s` | `IEventListenerV1sClient` | Register and manage event listeners |
| `EventV1s` | `IEventV1sClient` | Submit events (immediate or scheduled) |
| `ListenerEventV1s` | `IListenerEventV1sClient` | Query per-listener delivery results |

---

## Platform-Specific Examples

### Console Application

```csharp
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

class Program
{
    static async Task Main(string[] args)
    {
        var eventHighway = new EventHighwayClient(
            "Server=.;Database=EventHighwayDB;Trusted_Connection=True;TrustServerCertificate=True;");

        DateTimeOffset now = DateTimeOffset.UtcNow;

        var eventV1 = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = Guid.Parse("your-preconfigured-address-id"),
            Content = """{ "message": "Hello from Console" }""",
            Type = EventV1Type.Immediate,
            CreatedDate = now,
            UpdatedDate = now
        };

        EventV1 submitted = await eventHighway.EventV1s.SubmitEventV1Async(eventV1);

        Console.WriteLine($"Event submitted: {submitted.Id}");
    }
}
```

### ASP.NET Core Web API

```csharp
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("EventHighwayDB")
    ?? throw new InvalidOperationException("Missing EventHighwayDB connection string.");

builder.Services.AddSingleton<IEventHighwayClient>(
    new EventHighwayClient(connectionString));

var app = builder.Build();

app.MapPost("/api/events", async (
    EventV1 eventV1,
    IEventHighwayClient eventHighway) =>
{
    DateTimeOffset now = DateTimeOffset.UtcNow;
    eventV1.Id = Guid.NewGuid();
    eventV1.Type = EventV1Type.Immediate;
    eventV1.CreatedDate = now;
    eventV1.UpdatedDate = now;

    EventV1 submitted = await eventHighway.EventV1s.SubmitEventV1Async(eventV1);

    return Results.Created($"/api/events/{submitted.Id}", submitted);
});

app.MapGet("/api/events/{eventId:guid}/delivery", async (
    Guid eventId,
    IEventHighwayClient eventHighway) =>
{
    var listenerEvents = await eventHighway.ListenerEventV1s
        .RetrieveAllListenerEventV1sAsync();

    var results = listenerEvents
        .Where(le => le.EventId == eventId)
        .ToList();

    return Results.Ok(results);
});

app.Run();
```

### Worker Service

```csharp
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

public class EventPublisherWorker : BackgroundService
{
    private readonly IEventHighwayClient eventHighway;

    public EventPublisherWorker(IEventHighwayClient eventHighway) =>
        this.eventHighway = eventHighway;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var heartbeatEvent = new EventV1
            {
                Id = Guid.NewGuid(),
                EventAddressId = Guid.Parse("your-heartbeat-address-id"),
                Content = $"""{{ "timestamp": "{now:O}" }}""",
                Type = EventV1Type.Immediate,
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventHighway.EventV1s.SubmitEventV1Async(heartbeatEvent);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

Register in `Program.cs`:

```csharp
var builder = Host.CreateApplicationBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("EventHighwayDB")!;

builder.Services.AddSingleton<IEventHighwayClient>(
    new EventHighwayClient(connectionString));

builder.Services.AddHostedService<EventPublisherWorker>();

var host = builder.Build();
host.Run();
```

---

## Database Notes

- EventHighway uses **Entity Framework Core** with **SQL Server** (`UseSqlServer`).
- On first initialization, `StorageBroker` calls `Database.Migrate()` automatically — no manual migration step is needed.
- Tables created include: `EventAddressV1s`, `EventListenerV1s`, `EventV1s`, `ListenerEventV1s`, `EventV1Archives`, `ListenerEventV1Archives`, and their legacy V0 counterparts.

---

## What's Next

- [EventHighway API Reference](EventHighway-Apis.md)
- [Real-Life Sample A — GlobalFoodBank](Real-Life-Sample-A.md)
- [Real-Life Sample B — ABCTech School](Real-Life-Sample-B.md)
- [Real-Life Sample C — TodoTracker](Real-Life-Sample-C.md)
