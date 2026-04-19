# Real-Life Sample A — GlobalFoodBank (GFB)

GFB is a global food bank that receives donated surplus food goods and re-distributes them based on a country's regional needs. This sample demonstrates how to integrate EventHighway into a Standard-Compliant C# ASP.NET Core Web API backed by SQL Server.

> [!TIP]
> ![V1](https://img.shields.io/badge/V1_(v2.10+)-Recommended-brightgreen?style=for-the-badge)
> This sample uses the **V1** API surface.

---

## Premise

This sample demonstrates how to integrate **EventHighway** into a [Standard-Compliant](https://github.com/hassanhabib/The-Standard) C# ASP.NET Core Web API backed by SQL Server.

The API project already follows The Standard architecture and uses the typical Standard-Compliant package stack (RESTFulSense, Xeption, etc.). This guide does **not** cover creating the Standard-Compliant project itself — it focuses solely on wiring EventHighway into an existing one for event-driven communication.

---

## User Story

**GlobalFoodBank (GFB)** is a global food bank that receives donated surplus food goods and re-distributes them based on a country's regional needs.

> *As a GFB operator, when surplus goods are received at a warehouse, I want downstream services (regional distribution, expiration tracking) to be notified automatically so distribution can begin without manual coordination.*

---

## Domain at a Glance

| Concept | Description |
|---|---|
| **SurplusGood** | A donated food item received at a GFB warehouse |
| **Receipt** | The act of recording a surplus good into inventory |
| **Distribution** | Allocating received goods to a regional food bank |

Some goods are perishable (e.g., surplus apples have an `ExpirationDate`), others are non-perishable (e.g., surplus grains do not).

---

## Step 1 — Install EventHighway

```bash
dotnet add package EventHighway
```

---

## Step 2 — Register EventHighway at Startup

In the existing Standard-Compliant API project, register `EventHighwayClient` as a singleton in `Program.cs` (or wherever brokers are wired up):

```csharp
using EventHighway.Core.Clients.EventHighways;

// ...existing builder setup...

string eventHighwayConnectionString =
    builder.Configuration.GetConnectionString("EventHighwayDB")
        ?? throw new InvalidOperationException("Missing EventHighwayDB connection string.");

builder.Services.AddSingleton<IEventHighwayClient>(
    new EventHighwayClient(eventHighwayConnectionString));

// ...existing app.Run()...
```

> EventHighway manages its own database and migrations. Use a **separate** connection string from your main application database.

---

## Step 3 — Create the Event Broker

Following The Standard, external dependencies are wrapped in a broker. Create an `IEventBroker` that delegates to `IEventHighwayClient`:

```csharp
public interface IEventBroker
{
    ValueTask<EventAddressV1> RegisterEventAddressAsync(EventAddressV1 eventAddressV1);
    ValueTask<EventListenerV1> RegisterEventListenerAsync(EventListenerV1 eventListenerV1);
    ValueTask<EventV1> PublishEventAsync(EventV1 eventV1);
    ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync();
    ValueTask ArchiveProcessedEventsAsync();
}
```

```csharp
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

public class EventBroker : IEventBroker
{
    private readonly IEventHighwayClient eventHighwayClient;

    public EventBroker(IEventHighwayClient eventHighwayClient) =>
        this.eventHighwayClient = eventHighwayClient;

    public async ValueTask<EventAddressV1> RegisterEventAddressAsync(
        EventAddressV1 eventAddressV1) =>
            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(eventAddressV1);

    public async ValueTask<EventListenerV1> RegisterEventListenerAsync(
        EventListenerV1 eventListenerV1) =>
            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(eventListenerV1);

    public async ValueTask<EventV1> PublishEventAsync(EventV1 eventV1) =>
        await this.eventHighwayClient.EventV1s.SubmitEventV1Async(eventV1);

    public async ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync() =>
        await this.eventHighwayClient.ListenerEventV1s
            .RetrieveAllListenerEventV1sAsync();

    public async ValueTask ArchiveProcessedEventsAsync() =>
        await this.eventHighwayClient.EventV1sV1
            .ArchiveDeadEventV1sAsync();
}
```

Register the broker:

```csharp
builder.Services.AddTransient<IEventBroker, EventBroker>();
```

---

## Step 4 — Seed Event Addresses and Listeners

On application startup (or via a one-time seed), register the addresses and listeners that GFB needs.

GFB uses two event addresses:

| Address | Purpose |
|---|---|
| `GoodsReceived` | Fires when surplus goods arrive at a warehouse |
| `GoodsDistributed` | Fires when goods are allocated to a regional food bank |

```csharp
public class EventHighwaySeeder
{
    private readonly IEventBroker eventBroker;

    public EventHighwaySeeder(IEventBroker eventBroker) =>
        this.eventBroker = eventBroker;

    public async ValueTask SeedAsync()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        // ---- Addresses ----

        var goodsReceivedAddress = new EventAddressV1
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"),
            Name = "GoodsReceived",
            Description = "Surplus goods received at a GFB warehouse",
            CreatedDate = now,
            UpdatedDate = now
        };

        var goodsDistributedAddress = new EventAddressV1
        {
            Id = Guid.Parse("a1b2c3d4-0002-0002-0002-000000000002"),
            Name = "GoodsDistributed",
            Description = "Goods allocated to a regional food bank",
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.RegisterEventAddressAsync(goodsReceivedAddress);
        await this.eventBroker.RegisterEventAddressAsync(goodsDistributedAddress);

        // ---- Listeners ----

        var distributionListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Distribution Service",
            Description = "Triggers regional distribution when goods are received",
            HeaderSecret = "dist-secret-gfb-2026",
            Endpoint = "https://distribution.gfb.org/api/v1/events",
            EventAddressId = goodsReceivedAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        var expirationListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Expiration Tracker",
            Description = "Monitors perishable goods for expiration dates",
            HeaderSecret = "exp-secret-gfb-2026",
            Endpoint = "https://expiration.gfb.org/api/v1/events",
            EventAddressId = goodsReceivedAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        var auditListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Audit Service",
            Description = "Logs all distribution events for compliance",
            HeaderSecret = "audit-secret-gfb-2026",
            Endpoint = "https://audit.gfb.org/api/v1/events",
            EventAddressId = goodsDistributedAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.RegisterEventListenerAsync(distributionListener);
        await this.eventBroker.RegisterEventListenerAsync(expirationListener);
        await this.eventBroker.RegisterEventListenerAsync(auditListener);
    }
}
```

---

## Step 5 — API Controllers

GFB's Standard-Compliant API exposes RESTful controllers to the UI. Following The Standard, controllers are thin — they delegate all work to services and map exceptions to HTTP status codes via RESTFulSense.

### SurplusGoodsController

```csharp
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurplusGoodsController : RESTFulController
{
    private readonly ISurplusGoodOrchestrationService surplusGoodOrchestrationService;

    public SurplusGoodsController(
        ISurplusGoodOrchestrationService surplusGoodOrchestrationService) =>
            this.surplusGoodOrchestrationService = surplusGoodOrchestrationService;

    [HttpPost]
    public async ValueTask<ActionResult<SurplusGood>> PostSurplusGoodAsync(
        SurplusGood surplusGood)
    {
        SurplusGood receivedGood =
            await this.surplusGoodOrchestrationService
                .ReceiveSurplusGoodAsync(surplusGood);

        return Created(receivedGood);
    }
}
```

### DistributionsController

```csharp
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistributionsController : RESTFulController
{
    private readonly IDistributionOrchestrationService distributionOrchestrationService;

    public DistributionsController(
        IDistributionOrchestrationService distributionOrchestrationService) =>
            this.distributionOrchestrationService = distributionOrchestrationService;

    [HttpPost]
    public async ValueTask<ActionResult<Distribution>> PostDistributionAsync(
        Distribution distribution)
    {
        Distribution completedDistribution =
            await this.distributionOrchestrationService
                .ProcessDistributionAsync(distribution);

        return Created(completedDistribution);
    }
}
```

> In a full Standard-Compliant project, controllers wrap each action in a `TryCatch` that maps validation, dependency, and service exceptions to their corresponding HTTP status codes via RESTFulSense (e.g., `BadRequest`, `Conflict`, `InternalServerError`). Those mappings are omitted here for brevity.

### Orchestration Services

The controllers delegate to orchestration services. Orchestration services coordinate across multiple foundation services — they do **not** contain business logic or validation themselves.

```csharp
public class SurplusGoodOrchestrationService : ISurplusGoodOrchestrationService
{
    private readonly ISurplusGoodService surplusGoodService;
    private readonly ISurplusGoodEventService surplusGoodEventService;

    public SurplusGoodOrchestrationService(
        ISurplusGoodService surplusGoodService,
        ISurplusGoodEventService surplusGoodEventService)
    {
        this.surplusGoodService = surplusGoodService;
        this.surplusGoodEventService = surplusGoodEventService;
    }

    public async ValueTask<SurplusGood> ReceiveSurplusGoodAsync(SurplusGood surplusGood)
    {
        SurplusGood storedGood =
            await this.surplusGoodService.AddSurplusGoodAsync(surplusGood);

        await this.surplusGoodEventService
            .PublishGoodsReceivedEventAsync(storedGood);

        return storedGood;
    }
}
```

```csharp
public class DistributionOrchestrationService : IDistributionOrchestrationService
{
    private readonly IDistributionService distributionService;
    private readonly IDistributionEventService distributionEventService;

    public DistributionOrchestrationService(
        IDistributionService distributionService,
        IDistributionEventService distributionEventService)
    {
        this.distributionService = distributionService;
        this.distributionEventService = distributionEventService;
    }

    public async ValueTask<Distribution> ProcessDistributionAsync(
        Distribution distribution)
    {
        Distribution storedDistribution =
            await this.distributionService.AddDistributionAsync(distribution);

        await this.distributionEventService
            .PublishGoodsDistributedEventAsync(storedDistribution);

        return storedDistribution;
    }
}
```

### Request Flow

```
  UI (Browser/App)
       │
       ▼
  POST /api/surplusgoods ──► SurplusGoodsController
                                    │
                                    ▼
                         SurplusGoodOrchestrationService
                                    │
                         ┌──────────┴──────────┐
                         ▼                     ▼
                  SurplusGoodService    SurplusGoodEventService
                  (store to DB)        (publish GoodsReceived event)
                                              │
                                              ▼
                                        EventHighway V1
                                              │
                                    ┌─────────┴─────────┐
                                    ▼                   ▼
                             Distribution        Expiration
                               Service            Tracker
```

---

## Step 6 — Publish a Goods Received Event

When the warehouse API records a new surplus good, the orchestration service calls the event service below. The event service is a **Foundation-level** service.

### SurplusGood Model (simplified)

```csharp
public class SurplusGood
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double QuantityInKg { get; set; }
    public string WarehouseRegion { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public DateTimeOffset ReceivedDate { get; set; }
}
```

### SurplusGoodEventService

```csharp
public class SurplusGoodEventService : ISurplusGoodEventService
{
    private static readonly Guid GoodsReceivedAddressId =
        Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001");

    private readonly IEventBroker eventBroker;
    private readonly ISerializationBroker serializationBroker;
    private readonly IDateTimeBroker dateTimeBroker;

    public SurplusGoodEventService(
        IEventBroker eventBroker,
        ISerializationBroker serializationBroker,
        IDateTimeBroker dateTimeBroker)
    {
        this.eventBroker = eventBroker;
        this.serializationBroker = serializationBroker;
        this.dateTimeBroker = dateTimeBroker;
    }

    public async ValueTask PublishGoodsReceivedEventAsync(SurplusGood surplusGood)
    {
        ValidateSurplusGood(surplusGood);
        DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

        string content = this.serializationBroker.Serialize(new
        {
            surplusGood.Id,
            surplusGood.Name,
            surplusGood.Category,
            surplusGood.QuantityInKg,
            surplusGood.WarehouseRegion,
            surplusGood.ExpirationDate,
            surplusGood.ReceivedDate
        });

        var goodsReceivedEvent = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = GoodsReceivedAddressId,
            Content = content,
            Type = EventV1Type.Immediate,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.PublishEventAsync(goodsReceivedEvent);
    }

    private static void ValidateSurplusGood(SurplusGood surplusGood)
    {
        Validate(
            (Rule: IsInvalid(surplusGood.Id), Parameter: nameof(SurplusGood.Id)),
            (Rule: IsInvalid(surplusGood.Name), Parameter: nameof(SurplusGood.Name)),
            (Rule: IsInvalid(surplusGood.Category), Parameter: nameof(SurplusGood.Category)),
            (Rule: IsInvalid(surplusGood.WarehouseRegion), Parameter: nameof(SurplusGood.WarehouseRegion)));
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = "Id is invalid"
    };

    private static dynamic IsInvalid(string text) => new
    {
        Condition = string.IsNullOrWhiteSpace(text),
        Message = "Text is required"
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidSurplusGoodException =
            new InvalidSurplusGoodException(
                message: "Surplus good is invalid. Fix the errors and try again.");

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidSurplusGoodException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidSurplusGoodException.ThrowIfContainsErrors();
    }
}
```

### Example — Receiving Perishable Goods (Apples)

```csharp
var surplusApples = new SurplusGood
{
    Id = Guid.NewGuid(),
    Name = "Apples",
    Category = "Perishable",
    QuantityInKg = 500,
    WarehouseRegion = "EU-West",
    ExpirationDate = DateTimeOffset.UtcNow.AddDays(14),
    ReceivedDate = DateTimeOffset.UtcNow
};

await surplusGoodEventService.PublishGoodsReceivedEventAsync(surplusApples);
```

### Example — Receiving Non-Perishable Goods (Grains)

```csharp
var surplusGrains = new SurplusGood
{
    Id = Guid.NewGuid(),
    Name = "Rice Grains",
    Category = "Non-Perishable",
    QuantityInKg = 2000,
    WarehouseRegion = "APAC-South",
    ExpirationDate = null,
    ReceivedDate = DateTimeOffset.UtcNow
};

await surplusGoodEventService.PublishGoodsReceivedEventAsync(surplusGrains);
```

When either event fires, both the **Distribution Service** and the **Expiration Tracker** are notified automatically.

---

## Step 7 — Publish a Goods Distributed Event

After the distribution service allocates goods to a region, it publishes a distribution event so the audit service is notified. Like Step 6, this is a foundation-level event service with validation and broker-delegated serialization.

### Distribution Model (simplified)

```csharp
public class Distribution
{
    public Guid Id { get; set; }
    public Guid SurplusGoodId { get; set; }
    public string DestinationRegion { get; set; }
    public double QuantityInKg { get; set; }
    public DateTimeOffset DistributedDate { get; set; }
}
```

### DistributionEventService

```csharp
public class DistributionEventService : IDistributionEventService
{
    private static readonly Guid GoodsDistributedAddressId =
        Guid.Parse("a1b2c3d4-0002-0002-0002-000000000002");

    private readonly IEventBroker eventBroker;
    private readonly ISerializationBroker serializationBroker;
    private readonly IDateTimeBroker dateTimeBroker;

    public DistributionEventService(
        IEventBroker eventBroker,
        ISerializationBroker serializationBroker,
        IDateTimeBroker dateTimeBroker)
    {
        this.eventBroker = eventBroker;
        this.serializationBroker = serializationBroker;
        this.dateTimeBroker = dateTimeBroker;
    }

    public async ValueTask PublishGoodsDistributedEventAsync(Distribution distribution)
    {
        ValidateDistribution(distribution);
        DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

        string content = this.serializationBroker.Serialize(new
        {
            distribution.SurplusGoodId,
            distribution.DestinationRegion,
            distribution.QuantityInKg,
            distribution.DistributedDate
        });

        var distributionEvent = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = GoodsDistributedAddressId,
            Content = content,
            Type = EventV1Type.Immediate,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.PublishEventAsync(distributionEvent);
    }

    private static void ValidateDistribution(Distribution distribution)
    {
        Validate(
            (Rule: IsInvalid(distribution.SurplusGoodId), Parameter: nameof(Distribution.SurplusGoodId)),
            (Rule: IsInvalid(distribution.DestinationRegion), Parameter: nameof(Distribution.DestinationRegion)));
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = "Id is invalid"
    };

    private static dynamic IsInvalid(string text) => new
    {
        Condition = string.IsNullOrWhiteSpace(text),
        Message = "Text is required"
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidDistributionException =
            new InvalidDistributionException(
                message: "Distribution is invalid. Fix the errors and try again.");

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidDistributionException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidDistributionException.ThrowIfContainsErrors();
    }
}
```

### Example — Distributing Apples to a Region

```csharp
await distributionEventService.PublishGoodsDistributedEventAsync(
    new Distribution
    {
        Id = Guid.NewGuid(),
        SurplusGoodId = surplusApples.Id,
        DestinationRegion = "EU-West-Region-3",
        QuantityInKg = 200,
        DistributedDate = DateTimeOffset.UtcNow
    });
```

The **Audit Service** listener receives this event for compliance logging.

---

## Step 8 — Observe Delivery Results

After publishing, use the **Fire and Observe** pattern to verify each listener received the event. This is a foundation-level service that retrieves and filters listener event records.

```csharp
public class ListenerEventV1Service : IListenerEventV1Service
{
    private readonly IEventBroker eventBroker;

    public ListenerEventV1Service(IEventBroker eventBroker) =>
        this.eventBroker = eventBroker;

    public async ValueTask<List<ListenerEventV1>> RetrieveListenerEventV1sByEventIdAsync(
        Guid eventId)
    {
        ValidateEventId(eventId);

        IQueryable<ListenerEventV1> allListenerEvents =
            await this.eventBroker.RetrieveAllListenerEventsAsync();

        return allListenerEvents
            .Where(listenerEvent => listenerEvent.EventId == eventId)
            .ToList();
    }

    private static void ValidateEventId(Guid eventId)
    {
        if (eventId == Guid.Empty)
        {
            throw new InvalidListenerEventV1Exception(
                message: "ListenerEventV1 event id is invalid.");
        }
    }
}
```

### Example — Checking Delivery

```csharp
List<ListenerEventV1> results =
    await listenerEventV1Service
        .RetrieveListenerEventV1sByEventIdAsync(submittedEvent.Id);

foreach (ListenerEventV1 result in results)
{
    Console.WriteLine(
        $"Listener: {result.EventListenerId} | " +
        $"Status: {result.Status} | " +
        $"Response: {result.Response}");
}
```

Expected output when both listeners succeed:

```
Listener: <distribution-service-id> | Status: Success | Response: 200
Listener: <expiration-tracker-id>   | Status: Success | Response: 200
```

If a listener fails:

```
Listener: <distribution-service-id> | Status: Success | Response: 200
Listener: <expiration-tracker-id>   | Status: Error   | Response: 500
```

---

## Step 9 — Archive Processed Events

Once events have been delivered to all listeners and delivery results have been observed, the active `EventV1` and `ListenerEventV1` records are no longer needed for day-to-day operations. Archiving moves them into `EventV1Archive` and `ListenerEventV1Archive` tables for long-term audit and replay, keeping the active tables lean.

### How Archiving Works

When `ArchiveProcessedEventsAsync()` is called (which delegates to `ArchiveDeadEventV1sAsync()`), EventHighway:

1. Retrieves all **dead events** — events where every registered listener has a terminal status (`Success` or `Error`).
2. Maps each `EventV1` and its `ListenerEventV1` records into `EventV1Archive` and `ListenerEventV1Archive`, stamping an `ArchivedDate`.
3. Inserts the archive records.
4. Removes the original records from the active tables.

### ArchiveEventService

```csharp
public class ArchiveEventService : IArchiveEventService
{
    private readonly IEventBroker eventBroker;
    private readonly ILoggingBroker loggingBroker;

    public ArchiveEventService(
        IEventBroker eventBroker,
        ILoggingBroker loggingBroker)
    {
        this.eventBroker = eventBroker;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask ArchiveAllProcessedEventsAsync()
    {
        await this.eventBroker.ArchiveProcessedEventsAsync();

        this.loggingBroker.LogInformation(
            "Archived all processed events and listener results.");
    }
}
```

### Example — One-Time Archival

```csharp
await archiveEventService.ArchiveAllProcessedEventsAsync();
```

### Example — Periodic Archival on a Schedule

In a production GFB deployment, archiving can run on a timer so completed events are cleaned up automatically:

```csharp
public class ArchivalWorker : BackgroundService
{
    private readonly IArchiveEventService archiveEventService;

    public ArchivalWorker(IArchiveEventService archiveEventService) =>
        this.archiveEventService = archiveEventService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.archiveEventService.ArchiveAllProcessedEventsAsync();

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
```

### What Gets Archived

| Active Table | Archive Table | Key Fields Preserved |
|---|---|---|
| `EventV1` | `EventV1Archive` | `Id`, `Content`, `Type`, `CreatedDate`, `ScheduledDate` + `ArchivedDate` |
| `ListenerEventV1` | `ListenerEventV1Archive` | `Id`, `Status`, `Response`, `ResponseReasonPhrase`, `EventId`, `EventListenerId` + `ArchivedDate` |

After archival, the original `EventV1` and `ListenerEventV1` rows are **removed** from the active tables — they exist only in the archive from that point forward.

---



## Event Flow Summary

```
  Warehouse API                    EventHighway                     Downstream Services
  ─────────────                    ────────────                     ───────────────────

  Receive Apples ──► Publish ──►  GoodsReceived  ──► Distribution Service
       (perishable)    Event        Address            Expiration Tracker
                                       │
                                       ▼
                                  ListenerEventV1 records
                                  (Status per listener)

  Distribution    ──► Publish ──►  GoodsDistributed ──► Audit Service
  Service decides      Event        Address
  allocation                            │
                                        ▼
                                   ListenerEventV1 records
                                   (Status per listener)

  Archive         ──► ArchiveDeadEventV1sAsync()
  (periodic)              │
                          ├── EventV1 ──► EventV1Archive
                          └── ListenerEventV1 ──► ListenerEventV1Archive
                          (originals removed from active tables)
```

---

## What's Next

-  [Real-Life Sample B — ABCTech School](Real-Life-Sample-B.md)
-  [Real-Life Sample C — TodoTracker](Real-Life-Sample-C.md)
- [Installing EventHighway](Installing-EventHighway.md)
- [EventHighway API Reference](EventHighway-Apis.md)

---
***Last Updated: March 18, 2026******Last Updated: March 18, 2026***
