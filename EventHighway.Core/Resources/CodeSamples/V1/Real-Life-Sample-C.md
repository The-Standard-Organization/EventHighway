# Real-Life Sample C — TodoTracker Console App

TodoTracker is a personal productivity tool that tracks TODO items. When a TODO is completed it fires an immediate event; when a TODO is created with a future due date it schedules a reminder event. This sample demonstrates how to integrate EventHighway into a C# Console Application.

> [!TIP]
> ![V1](https://img.shields.io/badge/V1_(v2.10+)-Recommended-brightgreen?style=for-the-badge)
> This sample uses the **V1** API surface.

---

## Premise

This sample demonstrates how to integrate **EventHighway** into a C# Console Application backed by SQL Server.

The project uses the following packages:

| Package | Purpose |
|---|---|
| [EventHighway](https://www.nuget.org/packages/EventHighway) | Event-driven communication (V1) |

This is a Console Application — there are no controllers or API frameworks. The entry point is `Program.cs`, which delegates to services directly.

This guide focuses solely on wiring EventHighway into an existing Console Application for event-driven TODO tracking.

---

## User Story

**TodoTracker** is a console-based productivity tool for managing personal TODO items.

> *As a user, when I complete a TODO I want a completion event fired immediately so tracking services are notified. When I create a TODO with a future due date I want a reminder event scheduled so I am reminded at the right time.*

---

## Domain at a Glance

| Concept | Description |
|---|---|
| **TodoItem** | A task with a title, optional due date, and completion status |
| **Completion** | Marking a TODO as done — fires an immediate event |
| **Reminder** | A scheduled event that fires when a TODO's due date arrives |

---

## Step 1 — Install EventHighway

```bash
dotnet add package EventHighway
```

---

## Step 2 — Initialize EventHighway in Program.cs

In a Console Application there is no `builder.Services` pipeline. Create the `EventHighwayClient` directly and pass it into your services.

```csharp
using EventHighway.Core.Clients.EventHighways;

string connectionString =
    "Server=.;Database=TodoTrackerEventHighwayDB;Trusted_Connection=True;TrustServerCertificate=True;";

var eventHighway = new EventHighwayClient(connectionString);
```

> EventHighway manages its own database and migrations. Use a **separate** connection string from your application database.

---

## Step 3 — Create the Event Broker

External dependencies are wrapped in a broker. Create an `IEventBroker` that delegates to `IEventHighwayClient`:

```csharp
public interface IEventBroker
{
    ValueTask<EventAddressV1> RegisterEventAddressAsync(EventAddressV1 eventAddressV1);
    ValueTask<EventListenerV1> RegisterEventListenerAsync(EventListenerV1 eventListenerV1);
    ValueTask<EventV1> PublishEventAsync(EventV1 eventV1);
    ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync();
    ValueTask FireScheduledPendingEventsAsync();
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

    public async ValueTask FireScheduledPendingEventsAsync() =>
        await this.eventHighwayClient.EventV1s
            .FireScheduledPendingEventV1sAsync();
}
```

Wire it up in `Program.cs`:

```csharp
var eventBroker = new EventBroker(eventHighway);
```

---

## Step 4 — Seed Event Addresses and Listeners

On first run (or via a one-time seed), register the addresses and listeners that TodoTracker needs.

TodoTracker uses two event addresses:

| Address | Purpose |
|---|---|
| `TodoCompleted` | Fires immediately when a TODO is marked as done |
| `TodoReminder` | Fires at a scheduled time when a TODO's due date arrives |

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

        var todoCompletedAddress = new EventAddressV1
        {
            Id = Guid.Parse("c1d2e3f4-0001-0001-0001-000000000001"),
            Name = "TodoCompleted",
            Description = "A TODO item was marked as completed",
            CreatedDate = now,
            UpdatedDate = now
        };

        var todoReminderAddress = new EventAddressV1
        {
            Id = Guid.Parse("c1d2e3f4-0002-0002-0002-000000000002"),
            Name = "TodoReminder",
            Description = "A scheduled reminder for an upcoming TODO",
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.RegisterEventAddressAsync(todoCompletedAddress);
        await this.eventBroker.RegisterEventAddressAsync(todoReminderAddress);

        // ---- Listeners ----

        var completionLogListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Completion Log Service",
            Description = "Logs completed TODOs for productivity reporting",
            HeaderSecret = "log-secret-todo-2026",
            Endpoint = "https://todotracker.local/api/v1/completions",
            EventAddressId = todoCompletedAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        var reminderNotifyListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Reminder Notification Service",
            Description = "Sends a reminder notification when a TODO is due",
            HeaderSecret = "remind-secret-todo-2026",
            Endpoint = "https://todotracker.local/api/v1/reminders",
            EventAddressId = todoReminderAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.RegisterEventListenerAsync(completionLogListener);
        await this.eventBroker.RegisterEventListenerAsync(reminderNotifyListener);
    }
}
```

Wire it up in `Program.cs`:

```csharp
var seeder = new EventHighwaySeeder(eventBroker);
await seeder.SeedAsync();
```

---

## Step 5 — Console Entry Point (Program.cs)

In a Console Application there are no controllers. `Program.cs` is the entry point that wires brokers, seeds data, and delegates to services. It is the equivalent of a controller — thin and delegation-only.

```csharp
using EventHighway.Core.Clients.EventHighways;

string connectionString =
    "Server=.;Database=TodoTrackerEventHighwayDB;Trusted_Connection=True;TrustServerCertificate=True;";

var eventHighway = new EventHighwayClient(connectionString);
var eventBroker = new EventBroker(eventHighway);
var serializationBroker = new SerializationBroker();
var dateTimeBroker = new DateTimeBroker();

// Seed (one-time)
var seeder = new EventHighwaySeeder(eventBroker);
await seeder.SeedAsync();

// Wire services
var todoCompletedEventService =
    new TodoCompletedEventService(eventBroker, serializationBroker, dateTimeBroker);

var todoReminderEventService =
    new TodoReminderEventService(eventBroker, serializationBroker, dateTimeBroker);

var listenerEventV1Service = new ListenerEventV1Service(eventBroker);

// ---- Scenario: Complete a TODO ----

var buyGroceries = new TodoItem
{
    Id = Guid.NewGuid(),
    Title = "Buy groceries",
    DueDate = null,
    IsCompleted = true,
    CompletedDate = DateTimeOffset.UtcNow,
    CreatedDate = DateTimeOffset.UtcNow
};

EventV1 completedEvent =
    await todoCompletedEventService.PublishTodoCompletedEventAsync(buyGroceries);

Console.WriteLine($"Completed event published: {completedEvent.Id}");

// ---- Scenario: Schedule a reminder ----

var preparePresentation = new TodoItem
{
    Id = Guid.NewGuid(),
    Title = "Prepare quarterly presentation",
    DueDate = DateTimeOffset.UtcNow.AddDays(3),
    IsCompleted = false,
    CompletedDate = null,
    CreatedDate = DateTimeOffset.UtcNow
};

EventV1 scheduledEvent =
    await todoReminderEventService.PublishTodoReminderEventAsync(preparePresentation);

Console.WriteLine($"Reminder scheduled for: {preparePresentation.DueDate}");

// ---- Fire any due scheduled events ----

await eventBroker.FireScheduledPendingEventsAsync();

Console.WriteLine("Fired all due scheduled events.");

// ---- Observe delivery ----

List<ListenerEventV1> completionResults =
    await listenerEventV1Service
        .RetrieveListenerEventV1sByEventIdAsync(completedEvent.Id);

foreach (ListenerEventV1 result in completionResults)
{
    Console.WriteLine(
        $"  Listener: {result.EventListenerId} | " +
        $"Status: {result.Status} | " +
        $"Response: {result.Response}");
}
```

### Execution Flow

```
  Program.cs
       │
       ├── EventHighwayClient (init + auto-migrate)
       ├── EventBroker, SerializationBroker, DateTimeBroker
       ├── EventHighwaySeeder.SeedAsync()
       │
       ├── Complete a TODO
       │     └── TodoCompletedEventService.PublishTodoCompletedEventAsync()
       │           └── EventBroker.PublishEventAsync() ──► Completion Log Service
       │
       ├── Schedule a reminder
       │     └── TodoReminderEventService.PublishTodoReminderEventAsync()
       │           └── EventBroker.PublishEventAsync() ──► (pending until due)
       │
       ├── FireScheduledPendingEventsAsync()
       │     └── Fires due reminders ──► Reminder Notification Service
       │
       └── Observe delivery results
             └── ListenerEventV1Service.RetrieveListenerEventV1sByEventIdAsync()
```

---

## Step 6 — Publish a TODO Completed Event (Immediate)

When a TODO is marked as completed, an immediate event is fired. The event service is a **foundation-level** service — it validates its inputs and delegates serialization to a broker.

### Serialization Broker

Serialization is an external concern and belongs in a broker, not a service:

```csharp
public interface ISerializationBroker
{
    string Serialize<T>(T @object);
}
```

```csharp
using System.Text.Json;

public class SerializationBroker : ISerializationBroker
{
    public string Serialize<T>(T @object) =>
        JsonSerializer.Serialize(@object);
}
```

### TodoItem Model (simplified)

```csharp
public class TodoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
```

### TodoCompletedEventService

```csharp
public class TodoCompletedEventService : ITodoCompletedEventService
{
    private static readonly Guid TodoCompletedAddressId =
        Guid.Parse("c1d2e3f4-0001-0001-0001-000000000001");

    private readonly IEventBroker eventBroker;
    private readonly ISerializationBroker serializationBroker;
    private readonly IDateTimeBroker dateTimeBroker;

    public TodoCompletedEventService(
        IEventBroker eventBroker,
        ISerializationBroker serializationBroker,
        IDateTimeBroker dateTimeBroker)
    {
        this.eventBroker = eventBroker;
        this.serializationBroker = serializationBroker;
        this.dateTimeBroker = dateTimeBroker;
    }

    public async ValueTask<EventV1> PublishTodoCompletedEventAsync(TodoItem todoItem)
    {
        ValidateTodoItemOnCompleted(todoItem);
        DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

        string content = this.serializationBroker.Serialize(new
        {
            todoItem.Id,
            todoItem.Title,
            todoItem.CompletedDate
        });

        var todoCompletedEvent = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = TodoCompletedAddressId,
            Content = content,
            Type = EventV1Type.Immediate,
            CreatedDate = now,
            UpdatedDate = now
        };

        return await this.eventBroker.PublishEventAsync(todoCompletedEvent);
    }

    private static void ValidateTodoItemOnCompleted(TodoItem todoItem)
    {
        Validate(
            (Rule: IsInvalid(todoItem.Id), Parameter: nameof(TodoItem.Id)),
            (Rule: IsInvalid(todoItem.Title), Parameter: nameof(TodoItem.Title)),
            (Rule: IsNotCompleted(todoItem.IsCompleted), Parameter: nameof(TodoItem.IsCompleted)));
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

    private static dynamic IsNotCompleted(bool isCompleted) => new
    {
        Condition = isCompleted is false,
        Message = "TODO must be completed before publishing a completed event"
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidTodoItemException =
            new InvalidTodoItemException(
                message: "Todo item is invalid. Fix the errors and try again.");

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidTodoItemException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidTodoItemException.ThrowIfContainsErrors();
    }
}
```

### Example — Completing a TODO

```csharp
var buyGroceries = new TodoItem
{
    Id = Guid.NewGuid(),
    Title = "Buy groceries",
    DueDate = null,
    IsCompleted = true,
    CompletedDate = DateTimeOffset.UtcNow,
    CreatedDate = DateTimeOffset.UtcNow
};

EventV1 completedEvent =
    await todoCompletedEventService.PublishTodoCompletedEventAsync(buyGroceries);
```

The **Completion Log Service** listener is notified immediately.

---

## Step 7 — Publish a TODO Reminder Event (Scheduled)

When a TODO is created with a future `DueDate`, a **scheduled** event is published. The event remains pending until `FireScheduledPendingEventV1sAsync()` is called and the `ScheduledDate` has arrived.

### TodoReminderEventService

```csharp
public class TodoReminderEventService : ITodoReminderEventService
{
    private static readonly Guid TodoReminderAddressId =
        Guid.Parse("c1d2e3f4-0002-0002-0002-000000000002");

    private readonly IEventBroker eventBroker;
    private readonly ISerializationBroker serializationBroker;
    private readonly IDateTimeBroker dateTimeBroker;

    public TodoReminderEventService(
        IEventBroker eventBroker,
        ISerializationBroker serializationBroker,
        IDateTimeBroker dateTimeBroker)
    {
        this.eventBroker = eventBroker;
        this.serializationBroker = serializationBroker;
        this.dateTimeBroker = dateTimeBroker;
    }

    public async ValueTask<EventV1> PublishTodoReminderEventAsync(TodoItem todoItem)
    {
        ValidateTodoItemOnReminder(todoItem);
        DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

        string content = this.serializationBroker.Serialize(new
        {
            todoItem.Id,
            todoItem.Title,
            todoItem.DueDate
        });

        var todoReminderEvent = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = TodoReminderAddressId,
            Content = content,
            Type = EventV1Type.Scheduled,
            ScheduledDate = todoItem.DueDate,
            CreatedDate = now,
            UpdatedDate = now
        };

        return await this.eventBroker.PublishEventAsync(todoReminderEvent);
    }

    private static void ValidateTodoItemOnReminder(TodoItem todoItem)
    {
        Validate(
            (Rule: IsInvalid(todoItem.Id), Parameter: nameof(TodoItem.Id)),
            (Rule: IsInvalid(todoItem.Title), Parameter: nameof(TodoItem.Title)),
            (Rule: HasNoDueDate(todoItem.DueDate), Parameter: nameof(TodoItem.DueDate)));
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

    private static dynamic HasNoDueDate(DateTimeOffset? dueDate) => new
    {
        Condition = dueDate is null,
        Message = "Due date is required for scheduling a reminder"
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidTodoItemException =
            new InvalidTodoItemException(
                message: "Todo item is invalid. Fix the errors and try again.");

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidTodoItemException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidTodoItemException.ThrowIfContainsErrors();
    }
}
```

### Example — Scheduling a Reminder

```csharp
var preparePresentation = new TodoItem
{
    Id = Guid.NewGuid(),
    Title = "Prepare quarterly presentation",
    DueDate = DateTimeOffset.UtcNow.AddDays(3),
    IsCompleted = false,
    CompletedDate = null,
    CreatedDate = DateTimeOffset.UtcNow
};

EventV1 scheduledEvent =
    await todoReminderEventService.PublishTodoReminderEventAsync(preparePresentation);
```

The event is stored with `Type = EventV1Type.Scheduled` and `ScheduledDate` set to the TODO's `DueDate`. It will **not** fire until the scheduled time arrives and `FireScheduledPendingEventV1sAsync()` is called.

### Firing Due Scheduled Events

Call this periodically (e.g., on a timer, from a background loop, or on each console run):

```csharp
await eventBroker.FireScheduledPendingEventsAsync();
```

When the `ScheduledDate` has passed, the **Reminder Notification Service** listener is notified.

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

### Example — Checking Delivery After Completion

```csharp
List<ListenerEventV1> results =
    await listenerEventV1Service
        .RetrieveListenerEventV1sByEventIdAsync(completedEvent.Id);

foreach (ListenerEventV1 result in results)
{
    Console.WriteLine(
        $"Listener: {result.EventListenerId} | " +
        $"Status: {result.Status} | " +
        $"Response: {result.Response}");
}
```

Expected output:

```
Listener: <completion-log-service-id> | Status: Success | Response: 200
```

---

## Event Flow Summary

```
  Program.cs (Console App)            EventHighway                    Downstream Services
  ────────────────────────            ────────────                    ───────────────────

  Complete a TODO  ──► Publish ──►   TodoCompleted   ──► Completion Log Service
  (Immediate)           Event         Address             (logs for reporting)
                                         │
                                         ▼
                                    ListenerEventV1 records
                                    (Status per listener)

  Create a TODO    ──► Publish ──►   TodoReminder    ──► (pending until due)
  with DueDate          Event         Address
  (Scheduled)                            │
                                         ▼
                              FireScheduledPendingEventsAsync()
                                         │
                                         ▼
                                    Reminder Notification Service
                                    (sends reminder when due)
                                         │
                                         ▼
                                    ListenerEventV1 records
                                    (Status per listener)
```

---

## What's Next

- [Real-Life Sample A — GlobalFoodBank](Real-Life-Sample-A.md)
- [Real-Life Sample B — ABCTech School](Real-Life-Sample-B.md)
- [Installing EventHighway](Installing-EventHighway.md)
- [EventHighway API Reference](EventHighway-Apis.md)

---
***Last Updated: March 18, 2026***
