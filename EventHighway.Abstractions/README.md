![EventHighway](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/eventhighway-gitlogo.png)

[![BUILD](https://img.shields.io/github/actions/workflow/status/hassanhabib/EventHighway/build.yml?branch=main&label=EventHighway.Abstractions&logo=github)](https://github.com/hassanhabib/EventHighway/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EventHighway.Abstractions?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway.Abstractions)
[![Nuget](https://img.shields.io/nuget/dt/EventHighway.Abstractions?logo=nuget&style=default&color=blue&label=Downloads)](https://www.nuget.org/packages/EventHighway.Abstractions)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f?style=default)](https://github.com/hassanhabib/The-Standard)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard/releases/tag=latest)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)

# 0 - EventHighway.Abstractions

EventHighway.Abstractions is the contract library for [EventHighway](https://github.com/hassanhabib/EventHighway) — a Standard-Compliant .NET library for event-driven programming. This package contains only interfaces and lightweight models, so extension points (event handlers and storage providers) can be built against a small, stable surface without referencing the full engine.

# 1 - How It Works

The package provides two abstraction areas:

## 1.1 - Event Handler Abstractions

Everything needed to build an EventHighway-compatible event handler:

- `IEventHandler` — the contract EventHighway invokes to deliver an event to your code (`Id`, `Name` and `HandleAsync(content, cancellationToken)` returning an `EventHandlerResult`).
- `EventHandlerResult` — the outcome of handling an event (`IsSuccess`, `ResponseCode`, `ResponseMessage` and `Response`).
- Exception marker interfaces — `IEventHandlerValidationException`, `IEventHandlerDependencyException` and `IEventHandlerServiceException`.

> [!TIP]
> **You should not normally need to implement `IEventHandler` yourself.** A ready-made implementation, `DelegateEventHandler`, ships in the [EventHighway.EventHandlers](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.EventHandlers) package ([NuGet](https://www.nuget.org/packages/EventHighway.EventHandlers)). The only thing left to do is supply your own delegate — either directly in your domain, or exposed from a packaged delegate client library (see the [EventHighway.EventHandlers.Delegates.JoesRestApi](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.EventHandlers.Delegates.JoesRestApi) sample for the packaged pattern).

If you do implement your own `IEventHandler`, make sure the exceptions it throws implement the matching marker interface:

| Marker interface | Throw when |
|---|---|
| `IEventHandlerValidationException` | The event content or handler state is invalid |
| `IEventHandlerDependencyException` | An external dependency (downstream API, database, etc.) failed |
| `IEventHandlerServiceException` | An unexpected fault occurred inside the handler |

EventHighway uses these markers to categorize your exceptions correctly in its own exception handling and delivery records — exceptions that do not implement one of them are lumped together as service exceptions.

For example, a validation exception for your own handler would look like this:

```csharp
// YourDelegateEventHandlerValidationException.cs
using EventHighway.Abstractions.EventHandlers.Exceptions;

public class YourDelegateEventHandlerValidationException
    : Exception, IEventHandlerValidationException
{
    public YourDelegateEventHandlerValidationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
```

For reference, these are the exceptions `DelegateEventHandler` itself exposes — one per marker interface:

- [DelegateEventHandlerValidationException.cs](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.EventHandlers/Models/Exposers/DelegateEventHandlers/Exceptions/DelegateEventHandlerValidationException.cs)
- [DelegateEventHandlerDependencyException.cs](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.EventHandlers/Models/Exposers/DelegateEventHandlers/Exceptions/DelegateEventHandlerDependencyException.cs)
- [DelegateEventHandlerServiceException.cs](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.EventHandlers/Models/Exposers/DelegateEventHandlers/Exceptions/DelegateEventHandlerServiceException.cs)

## 1.2 - Storage Provider Abstractions

`IStorageBrokerProvider` is the contract for plugging a database engine into EventHighway:

```csharp
public interface IStorageBrokerProvider
{
    void Configure(DbContextOptionsBuilder optionsBuilder);
    void ConfigureModel(ModelBuilder modelBuilder);
}
```

A provider configures the EF Core connection (`Configure`) and applies any engine-specific model conventions (`ConfigureModel`). Two providers ship today, and you can implement your own for any other EF Core-supported engine:

| Provider | Project | NuGet |
|---|---|---|
| SQL Server | [EventHighway.SqlServer](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.SqlServer) | [EventHighway.SqlServer](https://www.nuget.org/packages/EventHighway.SqlServer) |
| PostgreSQL | [EventHighway.PostgreSql](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.PostgreSql) | [EventHighway.PostgreSql](https://www.nuget.org/packages/EventHighway.PostgreSql) |

# 2 - Installation

```bash
dotnet add package EventHighway.Abstractions
```

You only need a direct reference to this package when building your own `IEventHandler` or `IStorageBrokerProvider` implementation — the [EventHighway](https://www.nuget.org/packages/EventHighway) engine and its provider packages reference it transitively.

# 3 - Related Packages & Projects

| Package | Description |
|---|---|
| [EventHighway](https://github.com/hassanhabib/EventHighway) | The core event-driven engine ([NuGet](https://www.nuget.org/packages/EventHighway)) |
| [EventHighway.EventHandlers](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.EventHandlers) | Ready-made `IEventHandler` implementations, including `DelegateEventHandler` ([NuGet](https://www.nuget.org/packages/EventHighway.EventHandlers)) |
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


