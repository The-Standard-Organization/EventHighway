# EventHighway API Reference

All APIs are accessed through the `EventHighwayClient` entry point:

```csharp
var eventHighway = new EventHighwayClient(connectionString);
```

This document lists every available method grouped by client property.

---

## Version Overview

| Version | Client Properties | Status |
|---|---|---|
| V0 | `EventAddresses`, `EventListeners`, `Events` | ![Obsolete](https://img.shields.io/badge/Obsolete-red?style=flat-square) |
| V1 | `EventAddressV1s`, `EventListenerV1s`, `EventV1s`, `EventV1sV1`, `ListenerEventV1s` | ![Recommended](https://img.shields.io/badge/Recommended-brightgreen?style=flat-square) |

---

# V1 APIs (Recommended)

## EventAddressV1s — `IEventAddressesV1Client`

Manage event addresses that events are published to.

| Method | Signature | Description |
|---|---|---|
| **Register** | `RegisterEventAddressV1Async(EventAddressV1)` → `ValueTask<EventAddressV1>` | Create a new event address |
| **Retrieve All** | `RetrieveAllEventAddressV1sAsync()` → `ValueTask<IQueryable<EventAddressV1>>` | List all registered event addresses |
| **Remove** | `RemoveEventAddressV1ByIdAsync(Guid)` → `ValueTask<EventAddressV1>` | Delete an event address by Id |

### EventAddressV1 Model

```csharp
public class EventAddressV1
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}
```

### Example

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var address = new EventAddressV1
{
    Id = Guid.NewGuid(),
    Name = "OrderEvents",
    Description = "Address for order domain events",
    CreatedDate = now,
    UpdatedDate = now
};

EventAddressV1 registered = await eventHighway.EventAddressV1s
    .RegisterEventAddressV1Async(address);

IQueryable<EventAddressV1> allAddresses = await eventHighway.EventAddressV1s
    .RetrieveAllEventAddressV1sAsync();

EventAddressV1 removed = await eventHighway.EventAddressV1s
    .RemoveEventAddressV1ByIdAsync(registered.Id);
```

---

## EventListenerV1s — `IEventListenerV1sClient`

Register and manage listeners that receive events for a given address.

| Method | Signature | Description |
|---|---|---|
| **Register** | `RegisterEventListenerV1Async(EventListenerV1)` → `ValueTask<EventListenerV1>` | Register a new listener for an address |
| **Remove** | `RemoveEventListenerV1ByIdAsync(Guid)` → `ValueTask<EventListenerV1>` | Delete a listener by Id |

### EventListenerV1 Model

```csharp
public class EventListenerV1
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HeaderSecret { get; set; }
    public string Endpoint { get; set; }
    public Guid EventAddressId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}
```

### Example

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var listener = new EventListenerV1
{
    Id = Guid.NewGuid(),
    Name = "Shipping Service",
    Description = "Receives order events for shipment processing",
    HeaderSecret = "shipping-secret-token",
    Endpoint = "https://shipping.myapp.com/api/v1/events",
    EventAddressId = orderEventsAddressId,
    CreatedDate = now,
    UpdatedDate = now
};

EventListenerV1 registered = await eventHighway.EventListenerV1s
    .RegisterEventListenerV1Async(listener);

EventListenerV1 removed = await eventHighway.EventListenerV1s
    .RemoveEventListenerV1ByIdAsync(registered.Id);
```

---

## EventV1s — `IEventV1sClient`

Submit events and manage their lifecycle.

| Method | Signature | Description |
|---|---|---|
| **Submit** | `SubmitEventV1Async(EventV1)` → `ValueTask<EventV1>` | Submit an event for immediate or scheduled delivery |
| **Submit V1** | `SubmitEventV1AsyncV1(EventV1)` → `ValueTask<EventV1>` | Submit an event using the V1 coordination pipeline |
| **Fire Scheduled** | `FireScheduledPendingEventV1sAsync()` → `ValueTask` | Process all pending scheduled events that are due |
| **Remove** | `RemoveEventV1ByIdAsync(Guid)` → `ValueTask<EventV1>` | Delete an event by Id |

### EventV1 Model

```csharp
public class EventV1
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public EventV1Type Type { get; set; }          // Immediate | Scheduled
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? ScheduledDate { get; set; }
    public int RetryAttempts { get; set; }
    public Guid EventAddressId { get; set; }
}
```

### Example — Immediate Event

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var eventV1 = new EventV1
{
    Id = Guid.NewGuid(),
    EventAddressId = orderEventsAddressId,
    Content = """{ "orderId": "abc-123", "action": "created" }""",
    Type = EventV1Type.Immediate,
    CreatedDate = now,
    UpdatedDate = now
};

EventV1 submitted = await eventHighway.EventV1s.SubmitEventV1Async(eventV1);
```

### Example — Scheduled Event

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var scheduledEvent = new EventV1
{
    Id = Guid.NewGuid(),
    EventAddressId = orderEventsAddressId,
    Content = """{ "orderId": "abc-123", "action": "reminder" }""",
    Type = EventV1Type.Scheduled,
    ScheduledDate = now.AddHours(1),
    CreatedDate = now,
    UpdatedDate = now
};

EventV1 submitted = await eventHighway.EventV1s.SubmitEventV1Async(scheduledEvent);

// Later, fire all due scheduled events:
await eventHighway.EventV1s.FireScheduledPendingEventV1sAsync();
```

---

## Event Archiving — `IEventV1sClientV1` (`EventV1sV1`)

Archive management for fully-processed events. Archiving moves completed events and their per-listener delivery results out of the active tables and into dedicated archive tables for long-term audit, replay, and storage optimization.

| Method | Signature | Description |
|---|---|---|
| **Archive Dead Events** | `ArchiveDeadEventV1sAsync()` → `ValueTask` | Archive all fully-processed events and their listener results into `EventV1Archive` and `ListenerEventV1Archive`, then remove them from the active tables |

### How Archiving Works

When `ArchiveDeadEventV1sAsync()` is called, EventHighway:

1. Retrieves all **dead events** — events where every registered listener has a terminal status (`Success` or `Error`).
2. For each dead event, maps the `EventV1` and its `ListenerEventV1` records into their archive counterparts.
3. Inserts each `ListenerEventV1Archive` record, then inserts the `EventV1Archive` record.
4. Removes the original `ListenerEventV1` and `EventV1` records from the active tables.

This keeps the active tables lean while preserving the full delivery history in the archive.

### EventV1Archive Model

```csharp
public class EventV1Archive
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public EventV1ArchiveType Type { get; set; }       // Immediate | Scheduled
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset? ScheduledDate { get; set; }
    public DateTimeOffset ArchivedDate { get; set; }
    public Guid EventAddressId { get; set; }

    public IEnumerable<ListenerEventV1Archive> ListenerEventV1Archives { get; set; }
}
```

### ListenerEventV1Archive Model

```csharp
public class ListenerEventV1Archive
{
    public Guid Id { get; set; }
    public ListenerEventV1ArchiveStatus Status { get; set; }  // Pending | Success | Error
    public string Response { get; set; }
    public string ResponseReasonPhrase { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset ArchivedDate { get; set; }
    public Guid EventId { get; set; }
    public Guid EventAddressId { get; set; }
    public Guid EventListenerId { get; set; }
}
```

### Example — Archive All Completed Events

```csharp
// Archive all dead events (every listener has responded)
await eventHighway.EventV1sV1.ArchiveDeadEventV1sAsync();
```

### Example — Periodic Archival in a Worker Service

```csharp
public class ArchivalWorker : BackgroundService
{
    private readonly IEventHighwayClient eventHighway;

    public ArchivalWorker(IEventHighwayClient eventHighway) =>
        this.eventHighway = eventHighway;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.eventHighway.EventV1sV1.ArchiveDeadEventV1sAsync();

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
```

---

## ListenerEventV1s — `IListenerEventV1sClient`

Query and manage per-listener delivery results (the **Observe** side of Fire and Observe).

| Method | Signature | Description |
|---|---|---|
| **Retrieve All** | `RetrieveAllListenerEventV1sAsync()` → `ValueTask<IQueryable<ListenerEventV1>>` | List all listener event delivery records |
| **Remove** | `RemoveListenerEventV1ByIdAsync(Guid)` → `ValueTask<ListenerEventV1>` | Delete a listener event record by Id |

### ListenerEventV1 Model

```csharp
public class ListenerEventV1
{
    public Guid Id { get; set; }
    public ListenerEventV1Status Status { get; set; }  // Pending | Success | Error
    public string Response { get; set; }
    public string ResponseReasonPhrase { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid EventId { get; set; }
    public Guid EventAddressId { get; set; }
    public Guid EventListenerId { get; set; }
}
```

### Example

```csharp
IQueryable<ListenerEventV1> allResults = await eventHighway.ListenerEventV1s
    .RetrieveAllListenerEventV1sAsync();

// Filter delivery results for a specific event
var deliveryResults = allResults
    .Where(le => le.EventId == submittedEventId)
    .ToList();

foreach (var result in deliveryResults)
{
    Console.WriteLine(
        $"Listener: {result.EventListenerId} | " +
        $"Status: {result.Status} | " +
        $"Response: {result.Response} | " +
        $"Reason: {result.ResponseReasonPhrase}");
}

// Clean up a specific record
ListenerEventV1 removed = await eventHighway.ListenerEventV1s
    .RemoveListenerEventV1ByIdAsync(deliveryResults.First().Id);
```

---

# V0 APIs (Obsolete)

> [!NOTE]
> ![Obsolete](https://img.shields.io/badge/Version_0-Obsolete-red?style=for-the-badge)
> These APIs are from the initial release and follow a **Fire and Forget** model with no delivery observability. Use V1 APIs for all new development.

## EventAddresses — `IEventAddressesClient`

| Method | Signature | Description |
|---|---|---|
| **Register** | `RegisterEventAddressAsync(EventAddress)` → `ValueTask<EventAddress>` | Create a new event address |
| **Retrieve All** | `RetrieveAllEventAddressesAsync()` → `ValueTask<IQueryable<EventAddress>>` | List all event addresses |
| **Retrieve By Id** | `RetrieveEventAddressByIdAsync(Guid)` → `ValueTask<EventAddress>` | Retrieve a single event address |

## EventListeners — `IEventListenersClient`

| Method | Signature | Description |
|---|---|---|
| **Register** | `RegisterEventListenerAsync(EventListener)` → `ValueTask<EventListener>` | Register a listener |
| **Get All** | `GetAllEventListenersAsync()` → `ValueTask<IQueryable<EventListener>>` | List all listeners |

## Events — `IEventsClient`

| Method | Signature | Description |
|---|---|---|
| **Submit** | `SubmitEventAsync(Event)` → `ValueTask<Event>` | Publish an event (fire and forget) |

---

# Quick Reference — All Methods at a Glance

| Client Property | Method | Version |
|---|---|---|
| `EventAddressV1s` | `RegisterEventAddressV1Async` | V1 |
| `EventAddressV1s` | `RetrieveAllEventAddressV1sAsync` | V1 |
| `EventAddressV1s` | `RemoveEventAddressV1ByIdAsync` | V1 |
| `EventListenerV1s` | `RegisterEventListenerV1Async` | V1 |
| `EventListenerV1s` | `RemoveEventListenerV1ByIdAsync` | V1 |
| `EventV1s` | `SubmitEventV1Async` | V1 |
| `EventV1s` | `SubmitEventV1AsyncV1` | V1 |
| `EventV1s` | `FireScheduledPendingEventV1sAsync` | V1 |
| `EventV1s` | `RemoveEventV1ByIdAsync` | V1 |
| `EventV1sV1` | `ArchiveDeadEventV1sAsync` | V1 |
| `ListenerEventV1s` | `RetrieveAllListenerEventV1sAsync` | V1 |
| `ListenerEventV1s` | `RemoveListenerEventV1ByIdAsync` | V1 |
| `EventAddresses` | `RegisterEventAddressAsync` | V0 |
| `EventAddresses` | `RetrieveAllEventAddressesAsync` | V0 |
| `EventAddresses` | `RetrieveEventAddressByIdAsync` | V0 |
| `EventListeners` | `RegisterEventListenerAsync` | V0 |
| `EventListeners` | `GetAllEventListenersAsync` | V0 |
| `Events` | `SubmitEventAsync` | V0 |

---
***Last Updated: March 18, 2026***