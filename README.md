![EventHighway](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/eventhighway-gitlogo.png)

[![BUILD](https://github.com/hassanhabib/EventHighway/actions/workflows/build.yml/badge.svg)](https://github.com/hassanhabib/RESTFulSense/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EventHighway?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway)
[![Nuget](https://img.shields.io/nuget/dt/EventHighway?logo=nuget&style=default&color=blue&label=Downloads)](https://www.nuget.org/packages/EventHighway)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f?style=default)](https://github.com/hassanhabib/The-Standard)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard/releases/tag=latest)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)

# 0/ EventHighway

EventHighway is Standard-Compliant .NET library for event-driven programming. It is designed to be simple, lightweight, and easy to use.

## 0.1/ High-Level Flow

![High-Level Flow](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Diagrams/highlevel-flow.png)

## 0.2/ In-Depth Architecture

![In-Depth Architecture](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Diagrams/indepth-architecture.png)

# 0/ Version Information

Use the latest version when possible.

`Version 0` introduced a **Fire and Forget** model — publish events and move on.
`V1` (released in `v2.1.0`) evolves this into **Fire and Observe** — publish events and track what happened per listener with better visibility and operational confidence.

> [!NOTE]
> ![Obsolete](https://img.shields.io/badge/Version_0-Obsolete-red?style=for-the-badge)
> `Version 0` is the initial release and is now considered obsolete for new adoption. Emphasis is on `V1` for all new development, and existing users of `Version 0` are encouraged to upgrade to `V1` to benefit the new observability features.

> [!TIP]
> ![Recommended](https://img.shields.io/badge/V1_(v2.10+)-Recommended-brightgreen?style=for-the-badge)
> `V1` (released in `v2.10`) is the recommended version for teams that need observable, reliable event delivery.


# 1/ How to Use Basics

## 1.0/ Installation

You must define a connection string that points to a SQL DB Server when initializing the EventHighway client as follows:

```csharp
var eventHighway = new EventHighwayClient("Server=.;Database=EventHighwayDB;Trusted_Connection=True;");
```

---

## 1.1/ Registering Event Address

In order for an event to be published, it must target a certain `EventAddressV1`. You can register an `EventAddressV1` as follows:

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var eventAddressV1 = new EventAddressV1
{
	Id = Guid.NewGuid(),
	Name = "EventAddressName",
	Description = "EventAddressDescription",
	CreatedDate = now,
	UpdatedDate = now
};

await eventHighway.EventAddressV1s.RegisterEventAddressV1Async(eventAddressV1);
```

Make sure you store your `EventAddressV1` Id in a safe place, as you will need it to publish events to that address.

## 1.2/ Registering Event Listeners

In order to listen to events, you must register an `EventListenerV1` as follows:

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var eventListenerV1 = new EventListenerV1
{
	Id = Guid.NewGuid(),
	Name = "Students API Listener",
	Description = "Receives student domain events",
	HeaderSecret = "super-secret-token",
	Endpoint = "https://my.endpoint.com/api/v1.0/students",
	EventAddressId = SomePreconfiguredEventAddressId,
	CreatedDate = now,
	UpdatedDate = now
};

await eventHighway.EventListenerV1s.RegisterEventListenerV1Async(eventListenerV1);
```

## 1.3/ Publishing Events

You can publish an event as follows:

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var eventV1 = new EventV1
{
	Id = Guid.NewGuid(),
	EventAddressId = SomePreconfiguredEventAddressId,
	Content = "SomeStringifiedJsonContent",
	Type = EventV1Type.Immediate,
	CreatedDate = now,
	UpdatedDate = now
};

EventV1 submittedEventV1 = await eventHighway.EventV1s.SubmitEventV1Async(eventV1);

var listenerEvents = await eventHighway.ListenerEventV1s.RetrieveAllListenerEventV1sAsync();

var deliveryResults = listenerEvents
	.Where(listenerEventV1 => listenerEventV1.EventId == submittedEventV1.Id)
	.ToList();

```

When an event is submitted, notifications are sent to all registered `EventListenerV1` entries for that `EventAddressV1`. This is the `Fire and Observe` behavior, where you can query `ListenerEventV1` records to inspect delivery status per listener.

---

# Code Samples

In-depth, version-specific guides and real-world type examples

### V1 Guides

| Guide | Description |
|---|---|
| [Installing EventHighway](EventHighway.Core/Resources/CodeSamples/V1/Installing-EventHighway.md) | NuGet setup, supported platforms, initialization, and platform-specific examples |
| [EventHighway API Reference](EventHighway.Core/Resources/CodeSamples/V1/EventHighway-Apis.md) | Complete list of all available client methods, models, and usage examples |
| [Real-Life Sample A — GlobalFoodBank](EventHighway.Core/Resources/CodeSamples/V1/Real-Life-Sample-A.md) | End-to-end scenario: goods receipt, distribution events, and delivery observation |
| [Real-Life Sample B — ABCTech School](EventHighway.Core/Resources/CodeSamples/V1/Real-Life-Sample-B.md) | Azure-hosted scenario: class creation, student registration, and event-driven coordination |
| [Real-Life Sample C — TodoTracker](EventHighway.Core/Resources/CodeSamples/V1/Real-Life-Sample-C.md) | Console app scenario: immediate completion events, scheduled reminders, and Fire and Observe |

---
# Walk-through Video

[![YouTube EventHighway Introduction](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/YT/intro-eventhighway.jpg)](https://www.youtube.com/watch?v=z3_wx29Cs9U)

# Introduction to V1 Version
[![YouTube EventHighway v2.1.0 V1 Introduction](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/YT/v1-eventhighway.jpg)](https://www.youtube.com/watch?v=54YWf9G5cE8&t=1407s)

---

# Note

EventHighway is an officially released, Standard-Compliant Pub/Sub core library that can be deployed within an API or any Console Application. It is intentionally platform-agnostic so it can process events from anywhere to anywhere. With the introduction of `V1` (released in `v2.1.0`), the library moves beyond its initial fire-and-forget model to provide full observability, scheduled delivery, automatic archiving, and retry capabilities.

Current V1 capabilities include:

- **Immediate & Scheduled Events** — `EventV1Type.Immediate` for instant dispatch, `EventV1Type.Scheduled` for time-deferred delivery.
- **Delivery Observability** — per-listener `ListenerEventV1` records with `Pending`, `Success`, and `Error` statuses plus response details.
- **Automatic Archiving** — processed events and listener results are archived into `EventV1Archive` and `ListenerEventV1Archive` for audit and replay.
- **Retry Support** — configurable `RetryAttempts` on `EventV1` for resilient delivery.

There are plans for further abstraction and customization, such as:

- Enable plugging anything that implements `IStorageBroker` so consumers can use any storage mechanism or technology they prefer (e.g., PostgreSQL, CosmosDB, in-memory).
- Enable eventing beyond RESTful APIs — such as running the library within one microservice from Service to Service in a LakeHouse model, or supporting message-queue transports.
- Provide middleware hooks for custom serialization, filtering, and transformation of event payloads.

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
