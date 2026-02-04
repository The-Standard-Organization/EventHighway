
![EventHighway](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/eventhighway-gitlogo.png)

[![.NET](https://github.com/hassanhabib/EventHighway/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hassanhabib/RESTFulSense/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/EventHighway?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway)
[![Nuget](https://img.shields.io/nuget/dt/EventHighway?logo=nuget&style=default&color=blue&label=Downloads)](https://www.nuget.org/packages/EventHighway)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f?style=default)](https://github.com/hassanhabib/The-Standard)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?filter=v2.11.1&style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard/tree/2.11.1)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)

# 0/ EventHighway

EventHighway is Standard-Compliant .NET library for event-driven programming. It is designed to be simple, lightweight, and easy to use.

## 0.1/ High-Level Flow

![High-Level Flow](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Diagrams/highlevel-flow.png)

## 0.2/ In-Depth Architecture

![In-Depth Architecture](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Diagrams/indepth-architecture.png)

# 1/ How to Use

## 1.0/ Installation

You must define a connection string that points to a SQL DB Server when initializing the EventHighway client as follows:

```csharp
var eventHighway = new EventHighway("Server=.;Database=EventHighwayDB;Trusted_Connection=True;");
```

## 1.1/ Registering Event Address

In order for an event to be published, it must target a certain `EventAddress`. You can register an `EventAddress` as follows:

```csharp
var eventAddress = new EventAddress 
{
	Id = Guid.NewGuid(),
	Name = "EventAddressName",
	Description = "EventAddressDescription"
	CreatedDate = DateTimeOffset.UtcNow,
	UpdatedDate = DateTimeOffset.UtcNow
};

await eventHighway.EventAddresses.RegisterEventAddressAsync(eventAddress);
```

Make sure you store your `EventAddress` Id in a safe place, as you will need it to publish events to that address.

## 1.2/ Registering Event Listeners

In order to listen to events, you must register an `EventListener` as follows:

```csharp
var eventListener = new EventListener
{
	Id = Guid.NewGuid(),
	Endpoint = "https://my.endpoint.com/api/v1.0/students",
	EventAddressId = SomePreconfiguredEventAddressId,
	CreatedDate = DateTimeOffset.UtcNow,
	UpdatedDate = DateTimeOffset.UtcNow
};

await eventHighway.EventListeners.RegisterEventListenerAsync(eventListener);
```

## 1.3/ Publishing Events

You can publish an event as follows:

```csharp
var event = new Event
{
	Id = Guid.NewGuid(),
	EventAddressId = SomePreconfiguredEventAddressId,
	Content = "SomeStringifiedJsonContent",
	CreatedDate = DateTimeOffset.UtcNow,
	UpdatedDate = DateTimeOffset.UtcNow
};

await eventHighway.Events.PublishEventAsync(event);

```

When an event is published, a notification will be sent to all registered `EventListeners` that are listening to the event's `EventAddress`. A record of the status of the published event per listener will be available through the `ListenerEvent` table in the database.

# Walk-through Video

[![YouTube EventHighway Introduction](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/YT/intro-eventhighway.jpg)](https://www.youtube.com/watch?v=z3_wx29Cs9U)

# Note

This is an early release of a Pub/Sub pattern core library which can be deployed within an API or simple Console Application. It was intentionally built to be platform agnostic so it can process events from anywhere to anywhere.

There are plans for more abstraction and customization in the future, such as:

- Enable plugging anything that implements `IStorageBroker` so consumers can use any storage mechanism or technology they prefer.
- Enable eventing beyond RESTful APIs. Like running the library within one microservice from Service to Service in a LakeHouse model.

## Standard-Compliance
This library was built according to The Standard. The library follows engineering principles, patterns and tooling as recommended by The Standard.

This library is also a community effort which involved many nights of pair-programming, test-driven development and in-depth exploration research and design discussions.

## Standard-Promise
The most important fulfillment aspect in a Standard complaint system is aimed towards contributing to people, its evolution, and principles.
An organization that systematically honors an environment of learning, training, and sharing knowledge is an organization that learns from the past, makes calculated risks for the future, 
and brings everyone within it up to speed on the current state of things as honestly, rapidly, and efficiently as possible. 
 
We believe that everyone has the right to privacy, and will never do anything that could violate that right.
We are committed to writing ethical and responsible software, and will always strive to use our skills, coding, and systems for the good.
We believe that these beliefs will help to ensure that our software(s) are safe and secure and that it will never be used to harm or collect personal data for malicious purposes.
 
The Standard Community as a promise to you is in upholding these values.

