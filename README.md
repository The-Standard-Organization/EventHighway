![EventHighway](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/eventhighway-gitlogo.png)

[![BUILD](https://github.com/hassanhabib/EventHighway/actions/workflows/build.yml/badge.svg)](https://github.com/hassanhabib/RESTFulSense/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EventHighway?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway)
[![Nuget](https://img.shields.io/nuget/dt/EventHighway?logo=nuget&style=default&color=blue&label=Downloads)](https://www.nuget.org/packages/EventHighway)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f?style=default)](https://github.com/hassanhabib/The-Standard)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard/releases/tag=latest)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)

# 0 - EventHighway

EventHighway is Standard-Compliant .NET library for event-driven programming. It is designed to be simple, lightweight, and easy to use.

## 0.1 - How It Works

Publish an event once and it fans out to every matching listener, each producing a durable delivery record you can observe, retry, archive and replay. The full V2 flow — submission, filtering, loop detection, retries (Fibonacci backoff), archiving and replay — is documented in plain language here:

📖 **[EventHighway V2 — Design & Flow](Documentation/EventHighway.Core/V2/EventHighway.Core.V2.Design.md)**

## 0.2 - Operations Portal

A Standard-compliant Blazor Server console for operating an EventHighway installation: RAG health dashboards, event/archive browsing, bulk & single-item replay, participant and secret management, and user administration.

📖 **[EventHighway Operations Portal — Design & User Guide](Documentation/EventHighway.Portal.Web/EventHighway.Portal.Web.Design.md)**

![EventHighway Operations Portal — Health Status dashboard](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/Documentation/EventHighway.Portal.Web/Images/Portal-Dashboard-Status.png)

# 1 - Version Information

V2 Client is the latest version and should be used when possible. It is highly recommended to transition from previous versions to V2 for all new development, as support will end for older versions. The V2 client is available in the `v2.10` release and later.


> [!NOTE]
> ![Obsolete](https://img.shields.io/badge/Version_0_&_1-Obsolete-red?style=for-the-badge)
> `Version 0 / Version 1` is now considered obsolete for new adoption. Emphasis is on `V2` for all new development, and existing users of `Version 0 / Version 1` are encouraged to upgrade to `V2` to benefit from the new features.

> [!TIP]
> ![Recommended](https://img.shields.io/badge/V2_(v2.10+)-Recommended-brightgreen?style=for-the-badge)
> `V2` (released in `v2.10`) is the current recommended version for reliable event delivery.


# 2 - How to Use Basics (V2)

## 2.0 - Installation & Initialization

V2 runs against a SQL Server database. Construct the client with a storage provider and configuration — the database is created and migrated automatically on first use:

```csharp
var configuration = new EventHighwayConfiguration();

IClientV2 eventHighway = new EventHighwayClient(
	new SqlServerStorageBrokerProvider(
		"Server=.;Database=EventHighwayDB;Trusted_Connection=True;MultipleActiveResultSets=true"),
	configuration).V2;
```

---

## 2.1 - Registering a Handler

V2 delivers events to **in-process handlers** — no HTTP endpoint required. Register a handler by its stable `HandlerId`:

```csharp
var handler = new DelegateEventHandler(
	id: SomeStableHandlerId,
	handleAsync: (content, cancellationToken) =>
		ValueTask.FromResult(new EventHandlerResult { IsSuccess = true, ResponseCode = "200" }),
	name: "Students Handler");

eventHighway.RegisterEventHandler(handler);
```

## 2.2 - Registering an Address & Listener

Events target an `EventAddressV2` (a named channel); an `EventListenerV2` subscribes a handler to that address:

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

EventAddressV2 address = await eventHighway.EventAddressV2Client
	.RetrieveOrRegisterEventAddressV2Async(new EventAddressV2
	{
		Id = Guid.NewGuid(),
		Name = "students",
		Description = "Student domain events",
		CreatedDate = now,
		UpdatedDate = now
	});

await eventHighway.EventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(new EventListenerV2
{
	Id = Guid.NewGuid(),
	Name = "Students API Listener",
	HandlerId = handler.Id,
	HandlerName = handler.Name,
	EventAddressV2Id = address.Id,
	CreatedDate = now,
	UpdatedDate = now
});
```

A listener can optionally carry `PromotedProperties` + `FilterCriteria` so its handler only receives events that match (e.g. `double.Parse(meta("Rating")) >= 8.0`).

## 2.3 - Publishing & Observing

```csharp
await eventHighway.EventV2Client.SubmitEventV2Async(new EventV2
{
	Id = Guid.NewGuid(),
	EventAddressV2Id = address.Id,
	EventName = "StudentRegistered",
	Content = "{ \"studentId\": 123 }",
	// omit ScheduledDate for immediate dispatch; set it for a scheduled event
	CreatedDate = now,
	UpdatedDate = now
});

IQueryable<ListenerEventV2> deliveries =
	await eventHighway.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();
```

A published `EventV2` produces **one `ListenerEventV2` delivery record per matching listener** — each with a `Status` (`Pending` / `Success` / `Error` / `Replay`) and the handler's response. Scheduled events wait until you call `FireScheduledPendingEventV2sAsync()`. Events may be attributed to an `EventParticipantV2` and authorized with a participant secret.

---

# 3 - Code Samples

Runnable, end-to-end samples and in-depth guides:

| Sample | Description |
|---|---|
| [EventHighway V2 — Design & Flow](Documentation/EventHighway.Core/V2/EventHighway.Core.V2.Design.md) | Plain-language walkthrough of submit, dispatch, filtering, loop detection, retries, archiving and replay |
| `EventHighway.ClientV2.BasicApp` | A complete console sample: register handlers, publish immediate & scheduled events, filtering, loop detection, targeted replay, and a health summary |
| `EventHighway.ClientV2.SubstrateApp` | A service-to-service (in-process substrate) sample using the same V2 client |

> Legacy `V1` guides remain under [`EventHighway.Core/Resources/CodeSamples/V1`](EventHighway.Core/Resources/CodeSamples/V1) for existing users, but new development should target `V2`.

---
# Walk-through Video

TBC
---

# Note

EventHighway is an officially released, Standard-Compliant Pub/Sub core library that can be deployed within an API or any Console Application. It is intentionally platform-agnostic so it can process events from anywhere to anywhere. `V2` is an **in-process event bus with durable delivery records**, moving well beyond the original fire-and-forget model.

Current V2 capabilities include:

- **In-Process Handlers** — deliver events to registered `IEventHandler` code by `HandlerId`; no HTTP endpoint required.
- **Immediate & Scheduled Events** — instant dispatch, or time-deferred delivery fired by `FireScheduledPendingEventV2sAsync`.
- **Filtering & Promoted Properties** — optional - listeners receive only events matching their `FilterCriteria` if specified.
- **Delivery Observability** — one `ListenerEventV2` per (event × listener) with `Pending` / `Success` / `Error` / `Replay` status and the handler's response.
- **Loop Detection & Quarantine** — configurable thresholds guard against runaway republishing.
- **Participants & Secrets** — attribute and authorize events to an `EventParticipantV2` via rotating secrets.
- **Retries** — listener-level budgets with incremental (Fibonacci) backoff for resilient delivery.
- **Archiving & Replay** — processed events are archived, then replayed in bulk or to a targeted listener for back-fill.
- **Health & RAG Dashboards** — surfaced through the Operations Portal ([§0.2](#02-operations-portal)).
- **Pluggable Storage Providers** — anything implementing the storage-provider contract; SQL Server ships today.

---

## Standard-Compliance
This library was built according to The Standard. The library follows engineering principles, patterns and tooling as recommended by The Standard.

This library is also a community effort which involved many nights of pair-programming, test-driven development and in-depth exploration research and design discussions.

---

## Standard-Promise
The most important fulfillment aspect in a Standard compliant system is aimed towards contributing to people, its evolution, and principles.
An organization that systematically honors an environment of learning, training, and sharing knowledge is an organization that learns from the past, makes calculated risks for the future, 
and brings everyone within it up to speed on the current state of things as honestly, rapidly, and efficiently as possible. 
 
We believe that everyone has the right to privacy, and will never do anything that could violate that right.
We are committed to writing ethical and responsible software, and will always strive to use our skills, coding, and systems for the good.
We believe that these beliefs will help to ensure that our software(s) are safe and secure and that it will never be used to harm or collect personal data for malicious purposes.
 
The Standard Community as a promise to you is in upholding these values.

---

## Important Notice and Acknowledgements
A special thanks to all the community members, and the following dedicated engineers for their hard work and dedication to this project.
>Mr. Hassan Habib
>
>Mr. Christo du Toit
>
>Mr. Zafar Urakov
>
>Mr.Abdulsamad Osunlana
>
>Mr.Nodirkhan Abdumurotov
>
>Mr.Kailu Hu
>
>Mr.Greg Hays
>
>Mr.Ahmad Salim
