# Real-Life Sample B — ABCTech School

ABCTech is a school that manages teachers, students, and class registrations. This sample demonstrates how to integrate EventHighway into a Standard-Compliant C# ASP.NET Core Web API hosted on Azure, backed by Azure SQL and using ADotNet for data access.

> [!TIP]
> ![V1](https://img.shields.io/badge/V1_(v2.10+)-Recommended-brightgreen?style=for-the-badge)
> This sample uses the **V1** API surface.

---

## Premise

This sample demonstrates how to integrate **EventHighway** into a [Standard-Compliant](https://github.com/hassanhabib/The-Standard) C# ASP.NET Core Web API deployed to **Microsoft Azure**.

The API project already follows The Standard architecture by Hassan Habib and uses the Standard-Compliant package stack:

| Package | Purpose |
|---|---|
| [RESTFulSense](https://github.com/hassanhabib/RESTFulSense) | RESTful API controllers and exception mapping |
| [Xeption](https://github.com/hassanhabib/Xeption) | Meaningful, categorized exceptions |
| [ADotNet](https://github.com/hassanhabib/ADotNet) | Azure DevOps pipeline generation for CI/CD |
| [EventHighway](https://www.nuget.org/packages/EventHighway) | Event-driven communication (V1) |

The Azure infrastructure includes:

| Azure Resource | Role |
|---|---|
| **Azure App Service** | Hosts the ABCTech Web API |
| **Azure SQL Database** | Application database (teachers, students, classes) |
| **Azure SQL Database** | EventHighway database (separate instance) |

This guide does **not** cover creating the Standard-Compliant project or provisioning Azure resources — it focuses solely on wiring EventHighway into an existing one for event-driven communication between school domain services.

---

## User Story

**ABCTech** is a school where teachers create classes with specific classrooms, times, and dates, and students register for those classes.

> *As an ABCTech administrator, when a teacher creates a new class or a student registers for a class, I want downstream services (scheduling, notifications, capacity tracking) to be notified automatically so operations stay coordinated without manual intervention.*

---

## Domain at a Glance

| Concept | Description |
|---|---|
| **Teacher** | An instructor who creates and teaches classes |
| **Student** | A learner who registers for classes |
| **Classroom** | A physical room identified by building and room number |
| **SchoolClass** | A scheduled class taught by a teacher in a classroom at a specific date and time |
| **ClassRegistration** | A student's enrollment in a specific school class |

---

## Step 1 — Install EventHighway

```bash
dotnet add package EventHighway
```

---

## Step 2 — Register EventHighway at Startup

In the existing Standard-Compliant API project, register `EventHighwayClient` in `Program.cs`. On Azure, connection strings are stored in **App Service Configuration** and accessed through `IConfiguration`.

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

> EventHighway manages its own database and migrations against Azure SQL. Use a **separate** Azure SQL Database from your application database.

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
}
```

Register the broker:

```csharp
builder.Services.AddTransient<IEventBroker, EventBroker>();
```

---

## Step 4 — Seed Event Addresses and Listeners

On application startup (or via a one-time seed), register the addresses and listeners that ABCTech needs.

ABCTech uses two event addresses:

| Address | Purpose |
|---|---|
| `ClassCreated` | Fires when a teacher creates a new school class |
| `StudentRegistered` | Fires when a student registers for a class |

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

        var classCreatedAddress = new EventAddressV1
        {
            Id = Guid.Parse("b1c2d3e4-0001-0001-0001-000000000001"),
            Name = "ClassCreated",
            Description = "A teacher created a new school class",
            CreatedDate = now,
            UpdatedDate = now
        };

        var studentRegisteredAddress = new EventAddressV1
        {
            Id = Guid.Parse("b1c2d3e4-0002-0002-0002-000000000002"),
            Name = "StudentRegistered",
            Description = "A student registered for a school class",
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.RegisterEventAddressAsync(classCreatedAddress);
        await this.eventBroker.RegisterEventAddressAsync(studentRegisteredAddress);

        // ---- Listeners ----

        var schedulingListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Scheduling Service",
            Description = "Updates the master schedule when a class is created",
            HeaderSecret = "sched-secret-abctech-2026",
            Endpoint = "https://abctech-scheduling.azurewebsites.net/api/v1/events",
            EventAddressId = classCreatedAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        var notificationListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Notification Service",
            Description = "Sends confirmation to students upon registration",
            HeaderSecret = "notify-secret-abctech-2026",
            Endpoint = "https://abctech-notifications.azurewebsites.net/api/v1/events",
            EventAddressId = studentRegisteredAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        var capacityListener = new EventListenerV1
        {
            Id = Guid.NewGuid(),
            Name = "Capacity Tracker",
            Description = "Tracks classroom seat availability per class",
            HeaderSecret = "capacity-secret-abctech-2026",
            Endpoint = "https://abctech-capacity.azurewebsites.net/api/v1/events",
            EventAddressId = studentRegisteredAddress.Id,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.RegisterEventListenerAsync(schedulingListener);
        await this.eventBroker.RegisterEventListenerAsync(notificationListener);
        await this.eventBroker.RegisterEventListenerAsync(capacityListener);
    }
}
```

---

## Step 5 — API Controllers

ABCTech's Standard-Compliant API exposes RESTful controllers to the front-end. Controllers are thin — they delegate all work to services and map exceptions to HTTP status codes via RESTFulSense.

### SchoolClassesController

```csharp
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolClassesController : RESTFulController
{
    private readonly ISchoolClassOrchestrationService schoolClassOrchestrationService;

    public SchoolClassesController(
        ISchoolClassOrchestrationService schoolClassOrchestrationService) =>
            this.schoolClassOrchestrationService = schoolClassOrchestrationService;

    [HttpPost]
    public async ValueTask<ActionResult<SchoolClass>> PostSchoolClassAsync(
        SchoolClass schoolClass)
    {
        SchoolClass createdClass =
            await this.schoolClassOrchestrationService
                .CreateSchoolClassAsync(schoolClass);

        return Created(createdClass);
    }
}
```

### ClassRegistrationsController

```csharp
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassRegistrationsController : RESTFulController
{
    private readonly IClassRegistrationOrchestrationService classRegistrationOrchestrationService;

    public ClassRegistrationsController(
        IClassRegistrationOrchestrationService classRegistrationOrchestrationService) =>
            this.classRegistrationOrchestrationService = classRegistrationOrchestrationService;

    [HttpPost]
    public async ValueTask<ActionResult<ClassRegistration>> PostClassRegistrationAsync(
        ClassRegistration classRegistration)
    {
        ClassRegistration completedRegistration =
            await this.classRegistrationOrchestrationService
                .RegisterStudentAsync(classRegistration);

        return Created(completedRegistration);
    }
}
```

> In a full Standard-Compliant project, controllers wrap each action in a `TryCatch` that maps validation, dependency, and service exceptions to their corresponding HTTP status codes via RESTFulSense (e.g., `BadRequest`, `Conflict`, `InternalServerError`). Those mappings are omitted here for brevity.

### Orchestration Services

Orchestration services coordinate across multiple foundation services — they do **not** contain business logic or validation themselves.

```csharp
public class SchoolClassOrchestrationService : ISchoolClassOrchestrationService
{
    private readonly ISchoolClassService schoolClassService;
    private readonly ISchoolClassEventService schoolClassEventService;

    public SchoolClassOrchestrationService(
        ISchoolClassService schoolClassService,
        ISchoolClassEventService schoolClassEventService)
    {
        this.schoolClassService = schoolClassService;
        this.schoolClassEventService = schoolClassEventService;
    }

    public async ValueTask<SchoolClass> CreateSchoolClassAsync(SchoolClass schoolClass)
    {
        SchoolClass storedClass =
            await this.schoolClassService.AddSchoolClassAsync(schoolClass);

        await this.schoolClassEventService
            .PublishClassCreatedEventAsync(storedClass);

        return storedClass;
    }
}
```

```csharp
public class ClassRegistrationOrchestrationService : IClassRegistrationOrchestrationService
{
    private readonly IClassRegistrationService classRegistrationService;
    private readonly IClassRegistrationEventService classRegistrationEventService;

    public ClassRegistrationOrchestrationService(
        IClassRegistrationService classRegistrationService,
        IClassRegistrationEventService classRegistrationEventService)
    {
        this.classRegistrationService = classRegistrationService;
        this.classRegistrationEventService = classRegistrationEventService;
    }

    public async ValueTask<ClassRegistration> RegisterStudentAsync(
        ClassRegistration classRegistration)
    {
        ClassRegistration storedRegistration =
            await this.classRegistrationService
                .AddClassRegistrationAsync(classRegistration);

        await this.classRegistrationEventService
            .PublishStudentRegisteredEventAsync(storedRegistration);

        return storedRegistration;
    }
}
```

### Request Flow

```
  Front-End (Blazor / SPA)
       │
       ▼
  POST /api/schoolclasses ──► SchoolClassesController
                                     │
                                     ▼
                              SchoolClassOrchestrationService
                                     │
                          ┌──────────┴──────────┐
                          ▼                     ▼
                   SchoolClassService    SchoolClassEventService
                   (store to Azure SQL) (publish ClassCreated event)
                                               │
                                               ▼
                                         EventHighway V1
                                               │
                                               ▼
                                      Scheduling Service
                                   (Azure App Service)
```

---

## Step 6 — Publish a Class Created Event

When a teacher creates a new class, the orchestration service calls the event service below. The event service is a **foundation-level** service — it validates its inputs and delegates serialization to a broker.

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

### Models (simplified)

```csharp
public class Teacher
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}
```

```csharp
public class Classroom
{
    public Guid Id { get; set; }
    public string Building { get; set; }
    public string RoomNumber { get; set; }
    public int Capacity { get; set; }
}
```

```csharp
public class SchoolClass
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public Guid ClassroomId { get; set; }
    public DateTimeOffset ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}
```

### SchoolClassEventService

```csharp
public class SchoolClassEventService : ISchoolClassEventService
{
    private static readonly Guid ClassCreatedAddressId =
        Guid.Parse("b1c2d3e4-0001-0001-0001-000000000001");

    private readonly IEventBroker eventBroker;
    private readonly ISerializationBroker serializationBroker;
    private readonly IDateTimeBroker dateTimeBroker;

    public SchoolClassEventService(
        IEventBroker eventBroker,
        ISerializationBroker serializationBroker,
        IDateTimeBroker dateTimeBroker)
    {
        this.eventBroker = eventBroker;
        this.serializationBroker = serializationBroker;
        this.dateTimeBroker = dateTimeBroker;
    }

    public async ValueTask PublishClassCreatedEventAsync(SchoolClass schoolClass)
    {
        ValidateSchoolClass(schoolClass);
        DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

        string content = this.serializationBroker.Serialize(new
        {
            schoolClass.Id,
            schoolClass.Name,
            schoolClass.TeacherId,
            schoolClass.ClassroomId,
            schoolClass.ScheduledDate,
            schoolClass.StartTime,
            schoolClass.EndTime
        });

        var classCreatedEvent = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = ClassCreatedAddressId,
            Content = content,
            Type = EventV1Type.Immediate,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.PublishEventAsync(classCreatedEvent);
    }

    private static void ValidateSchoolClass(SchoolClass schoolClass)
    {
        Validate(
            (Rule: IsInvalid(schoolClass.Id), Parameter: nameof(SchoolClass.Id)),
            (Rule: IsInvalid(schoolClass.Name), Parameter: nameof(SchoolClass.Name)),
            (Rule: IsInvalid(schoolClass.TeacherId), Parameter: nameof(SchoolClass.TeacherId)),
            (Rule: IsInvalid(schoolClass.ClassroomId), Parameter: nameof(SchoolClass.ClassroomId)));
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
        var invalidSchoolClassException =
            new InvalidSchoolClassException(
                message: "School class is invalid. Fix the errors and try again.");

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidSchoolClassException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidSchoolClassException.ThrowIfContainsErrors();
    }
}
```

### Example — Creating a Class

```csharp
var introToCS = new SchoolClass
{
    Id = Guid.NewGuid(),
    Name = "Introduction to Computer Science",
    Description = "Foundational CS concepts for first-year students",
    TeacherId = teacherJohnsonId,
    ClassroomId = roomA101Id,
    ScheduledDate = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero),
    StartTime = new TimeSpan(9, 0, 0),
    EndTime = new TimeSpan(10, 30, 0),
    CreatedDate = DateTimeOffset.UtcNow,
    UpdatedDate = DateTimeOffset.UtcNow
};

await schoolClassEventService.PublishClassCreatedEventAsync(introToCS);
```

The **Scheduling Service** listener is notified to update the master timetable.

---

## Step 7 — Publish a Student Registered Event

When a student registers for a class, the orchestration service calls the event service below. The notification and capacity services are notified automatically.

### ClassRegistration Model (simplified)

```csharp
public class ClassRegistration
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid SchoolClassId { get; set; }
    public DateTimeOffset RegisteredDate { get; set; }
}
```

### Student Model (simplified)

```csharp
public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTimeOffset EnrolledDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}
```

### ClassRegistrationEventService

```csharp
public class ClassRegistrationEventService : IClassRegistrationEventService
{
    private static readonly Guid StudentRegisteredAddressId =
        Guid.Parse("b1c2d3e4-0002-0002-0002-000000000002");

    private readonly IEventBroker eventBroker;
    private readonly ISerializationBroker serializationBroker;
    private readonly IDateTimeBroker dateTimeBroker;

    public ClassRegistrationEventService(
        IEventBroker eventBroker,
        ISerializationBroker serializationBroker,
        IDateTimeBroker dateTimeBroker)
    {
        this.eventBroker = eventBroker;
        this.serializationBroker = serializationBroker;
        this.dateTimeBroker = dateTimeBroker;
    }

    public async ValueTask PublishStudentRegisteredEventAsync(
        ClassRegistration classRegistration)
    {
        ValidateClassRegistration(classRegistration);
        DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

        string content = this.serializationBroker.Serialize(new
        {
            classRegistration.Id,
            classRegistration.StudentId,
            classRegistration.SchoolClassId,
            classRegistration.RegisteredDate
        });

        var studentRegisteredEvent = new EventV1
        {
            Id = Guid.NewGuid(),
            EventAddressId = StudentRegisteredAddressId,
            Content = content,
            Type = EventV1Type.Immediate,
            CreatedDate = now,
            UpdatedDate = now
        };

        await this.eventBroker.PublishEventAsync(studentRegisteredEvent);
    }

    private static void ValidateClassRegistration(ClassRegistration classRegistration)
    {
        Validate(
            (Rule: IsInvalid(classRegistration.Id), Parameter: nameof(ClassRegistration.Id)),
            (Rule: IsInvalid(classRegistration.StudentId), Parameter: nameof(ClassRegistration.StudentId)),
            (Rule: IsInvalid(classRegistration.SchoolClassId), Parameter: nameof(ClassRegistration.SchoolClassId)));
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = "Id is invalid"
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidClassRegistrationException =
            new InvalidClassRegistrationException(
                message: "Class registration is invalid. Fix the errors and try again.");

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidClassRegistrationException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidClassRegistrationException.ThrowIfContainsErrors();
    }
}
```

### Example — Registering a Student for a Class

```csharp
var registration = new ClassRegistration
{
    Id = Guid.NewGuid(),
    StudentId = studentJaneDoeId,
    SchoolClassId = introToCS.Id,
    RegisteredDate = DateTimeOffset.UtcNow
};

await classRegistrationEventService
    .PublishStudentRegisteredEventAsync(registration);
```

Both the **Notification Service** (sends a confirmation email to the student) and the **Capacity Tracker** (decrements available seats) are notified automatically.

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

### Example — Checking Delivery After Student Registration

```csharp
List<ListenerEventV1> results =
    await listenerEventV1Service
        .RetrieveListenerEventV1sByEventIdAsync(studentRegisteredEvent.Id);

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
Listener: <notification-service-id> | Status: Success | Response: 200
Listener: <capacity-tracker-id>     | Status: Success | Response: 200
```

If a listener fails:

```
Listener: <notification-service-id> | Status: Success | Response: 200
Listener: <capacity-tracker-id>     | Status: Error   | Response: 500
```

---

## Event Flow Summary

```
  ABCTech API (Azure App Service)     EventHighway           Downstream (Azure App Services)
  ───────────────────────────────     ────────────           ───────────────────────────────

  Teacher creates    ──► Publish ──►  ClassCreated   ──► Scheduling Service
  a SchoolClass           Event        Address            (updates master timetable)
                                          │
                                          ▼
                                     ListenerEventV1 records
                                     (Status per listener)

  Student registers  ──► Publish ──►  StudentRegistered ──► Notification Service
  for a SchoolClass       Event        Address               (sends confirmation email)
                                          │               ──► Capacity Tracker
                                          ▼                   (decrements available seats)
                                     ListenerEventV1 records
                                     (Status per listener)
```

---

## Azure Deployment Notes

- **EventHighway's Azure SQL Database** is separate from ABCTech's application database. Configure two connection strings in App Service Configuration:
  - `ConnectionStrings__DefaultDB` — application data (teachers, students, classes)
  - `ConnectionStrings__EventHighwayDB` — EventHighway tables and migrations
- **Listener endpoints** point to other Azure App Services (e.g., `abctech-scheduling.azurewebsites.net`). Each downstream service receives events via HTTP POST with the `X-Highway` header containing the listener's `HeaderSecret`.
- **ADotNet** generates the Azure DevOps YAML pipeline for CI/CD. The build and deploy pipeline for the ABCTech API does not need any EventHighway-specific steps — the library's auto-migration handles schema updates on startup.

---

## What's Next

- [Real-Life Sample A — GlobalFoodBank](Real-Life-Sample-A.md)
- [Real-Life Sample C — TodoTracker](Real-Life-Sample-C.md)
- [Installing EventHighway](Installing-EventHighway.md)
- [EventHighway API Reference](EventHighway-Apis.md)
