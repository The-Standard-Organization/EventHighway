![EventHighway](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/eventhighway-gitlogo.png)

[![BUILD](https://img.shields.io/github/actions/workflow/status/hassanhabib/EventHighway/build.yml?branch=main&label=EventHighway.EventHandlers&logo=github)](https://github.com/hassanhabib/EventHighway/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EventHighway.EventHandlers?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway.EventHandlers)
[![Nuget](https://img.shields.io/nuget/dt/EventHighway.EventHandlers?logo=nuget&style=default&color=blue&label=Downloads)](https://www.nuget.org/packages/EventHighway.EventHandlers)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f?style=default)](https://github.com/hassanhabib/The-Standard)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard/releases/tag=latest)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)

# 0 - EventHighway.EventHandlers

EventHighway.EventHandlers provides ready-made `IEventHandler` implementations for [EventHighway](https://github.com/hassanhabib/EventHighway) — a Standard-Compliant .NET library for event-driven programming. Instead of implementing the [`IEventHandler`](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.Abstractions/EventHandlers/IEventHandler.cs) contract yourself, use the handlers in this package and plug in your own logic.

# 1 - How It Works

## 1.1 - DelegateEventHandler

`DelegateEventHandler` wraps any delegate of the shape `Func<string, CancellationToken, ValueTask<EventHandlerResult>>` in a fully Standard-compliant event handler — validation, exception mapping and categorization are all handled for you:

```csharp
IEventHandler handler = new DelegateEventHandler(
    someStableHandlerId,
    (content, cancellationToken) => ValueTask.FromResult(
        new EventHandlerResult { IsSuccess = true, ResponseCode = "200" }),
    name: "Students Handler");

eventHighway.RegisterEventHandler(handler);
```

- **Id** — a stable `Guid` identifying the handler; event listeners subscribe to it by this `HandlerId`, so keep it constant across restarts and deployments.
- **handler** — your delegate; it receives the raw event content and a cancellation token, and returns an `EventHandlerResult`.
- **name** — optional; defaults to `DelegateEventHandler`. Every handler registered with a client must have a unique name, so supply one when registering more than one delegate handler.

Exceptions thrown by your delegate are caught and re-thrown as one of the marker-interface exceptions EventHighway uses to categorize failures in its delivery records:

- [DelegateEventHandlerValidationException.cs](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.EventHandlers/Models/Exposers/DelegateEventHandlers/Exceptions/DelegateEventHandlerValidationException.cs)
- [DelegateEventHandlerDependencyException.cs](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.EventHandlers/Models/Exposers/DelegateEventHandlers/Exceptions/DelegateEventHandlerDependencyException.cs)
- [DelegateEventHandlerServiceException.cs](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.EventHandlers/Models/Exposers/DelegateEventHandlers/Exceptions/DelegateEventHandlerServiceException.cs)

## 1.2 - Packaging Your Delegate as a Client Library

Your delegate can live directly in your domain, or ship as a small per-integration satellite library that exposes a **delegate-compatible client** — a class with a method matching the delegate signature above. The consuming application then wires that method into a `DelegateEventHandler`, keeping the handler identity (`Id` and `name`) on the application side:

```csharp
services.AddJoesRestApiDelegateClient(configuration);

// later, when registering handlers:
IEventHandler handler = new DelegateEventHandler(
    joesHandlerId,
    joesRestApiDelegateClient.PostToJoesRestApiAsync,
    name: "Joes REST API");
```

See [EventHighway.EventHandlers.Delegates.JoesRestApi](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.EventHandlers.Delegates.JoesRestApi) for a complete sample of the packaged pattern.

# 2 - Installation

```bash
dotnet add package EventHighway.EventHandlers
```

# 3 - Related Packages & Projects

| Package | Description |
|---|---|
| [EventHighway](https://github.com/hassanhabib/EventHighway) | The core event-driven engine ([NuGet](https://www.nuget.org/packages/EventHighway)) |
| [EventHighway.Abstractions](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.Abstractions) | The `IEventHandler` and `IStorageBrokerProvider` contracts this package implements ([NuGet](https://www.nuget.org/packages/EventHighway.Abstractions)) |
| [EventHighway.SqlServer](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.SqlServer) | SQL Server storage provider ([NuGet](https://www.nuget.org/packages/EventHighway.SqlServer)) |
| [EventHighway.PostgreSql](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.PostgreSql) | PostgreSQL storage provider ([NuGet](https://www.nuget.org/packages/EventHighway.PostgreSql)) |

---

# 4 - Standard-Compliance
This library was built according to The Standard. The library follows engineering principles, patterns and tooling as recommended by The Standard.

This library is also a community effort which involved many nights of pair-programming, test-driven development and in-depth exploration research and design discussions.

---

# 5 - Standard-Promise
The most important fulfillment aspect in a Standard compliant system is aimed towards contributing to people, its evolution, and principles.
An organization that systematically honors an environment of learning, training, and sharing knowledge is an organization that learns from the past, makes calculated risks for the future, 
and brings everyone within it up to speed on the current state of things as honestly, rapidly, and efficiently as possible. 
 
We believe that everyone has the right to privacy, and will never do anything that could violate that right.
We are committed to writing ethical and responsible software, and will always strive to use our skills, coding, and systems for the good.
We believe that these beliefs will help to ensure that our software(s) are safe and secure and that it will never be used to harm or collect personal data for malicious purposes.
 
The Standard Community as a promise to you is in upholding these values.

---

# 6 - Important Notice and Acknowledgements
A special thanks to all the community members, and the following dedicated engineers for their hard work and dedication to this project.
>Mr. Hassan Habib
>
>Mr. Christo du Toit
>
>Mr.Ahmad Salim
>
>Mr.Greg Hays