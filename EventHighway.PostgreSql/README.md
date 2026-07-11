![EventHighway](https://raw.githubusercontent.com/hassanhabib/EventHighway/refs/heads/main/EventHighway.Core/Resources/Images/eventhighway-gitlogo.png)

[![BUILD](https://img.shields.io/github/actions/workflow/status/hassanhabib/EventHighway/build.yml?branch=main&label=EventHighway.PostgreSql&logo=github)](https://github.com/hassanhabib/EventHighway/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/EventHighway.PostgreSql?logo=nuget&style=default)](https://www.nuget.org/packages/EventHighway.PostgreSql)
[![Nuget](https://img.shields.io/nuget/dt/EventHighway.PostgreSql?logo=nuget&style=default&color=blue&label=Downloads)](https://www.nuget.org/packages/EventHighway.PostgreSql)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f?style=default)](https://github.com/hassanhabib/The-Standard)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard/releases/tag=latest)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)

# 0 - EventHighway.PostgreSql

EventHighway.PostgreSql is the **PostgreSQL storage provider** for [EventHighway](https://github.com/hassanhabib/EventHighway) — a Standard-Compliant .NET library for event-driven programming. It implements the [`IStorageBrokerProvider`](https://github.com/hassanhabib/EventHighway/blob/main/EventHighway.Abstractions/Storages/IStorageBrokerProvider.cs) contract from [EventHighway.Abstractions](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.Abstractions), persisting all EventHighway state — events, addresses, listeners, delivery records, participants and archives — to PostgreSQL via EF Core (Npgsql).

# 1 - How It Works

## 1.1 - Initializing the Client

Construct the EventHighway client with a `PostgreSqlStorageBrokerProvider` and your connection string — the database is created and migrated automatically on first use:

```csharp
var configuration = new EventHighwayConfiguration();

IClientV2 eventHighway = new EventHighwayClient(
    new PostgreSqlStorageBrokerProvider(
        "Host=localhost;Port=5432;Database=EventHighwayDB;Username=postgres;Password=postgres"),
    configuration).V2;
```

From here on, usage is identical regardless of provider — see the [EventHighway usage guide](https://github.com/hassanhabib/EventHighway#2---how-to-use-basics-v2) for registering handlers, addresses, listeners, and publishing events.

## 1.2 - Timestamp Precision

All `DateTimeOffset` properties are stored as `timestamptz` with microsecond precision. PostgreSQL rounds the 100-nanosecond tick that .NET carries, so the provider truncates values to whole microseconds on write — timestamps round-trip identically to the in-memory value, and behave the same as on the SQL Server provider.

## 1.3 - Migrations

This package embeds the full set of EF Core migrations for the EventHighway schema, applied automatically at startup — no manual scripts to run. Provider schemas are kept identical across PostgreSQL and SQL Server; if you are contributing a change that touches the database, a matching migration must be added to **both** provider projects — see the [Contributing guide](https://github.com/hassanhabib/EventHighway/blob/main/README.md#4---contributing) for the exact commands.

# 2 - Installation

```bash
dotnet add package EventHighway.PostgreSql
```

This brings in the EventHighway engine transitively — it is the only package you need to install to run EventHighway on PostgreSQL.

# 3 - Related Packages & Projects

| Package | Description |
|---|---|
| [EventHighway](https://github.com/hassanhabib/EventHighway) | The core event-driven engine ([NuGet](https://www.nuget.org/packages/EventHighway)) |
| [EventHighway.SqlServer](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.SqlServer) | SQL Server storage provider — the sibling of this package ([NuGet](https://www.nuget.org/packages/EventHighway.SqlServer)) |
| [EventHighway.Abstractions](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.Abstractions) | The `IStorageBrokerProvider` contract this package implements ([NuGet](https://www.nuget.org/packages/EventHighway.Abstractions)) |
| [EventHighway.EventHandlers](https://github.com/hassanhabib/EventHighway/tree/main/EventHighway.EventHandlers) | Ready-made `IEventHandler` implementations, including `DelegateEventHandler` ([NuGet](https://www.nuget.org/packages/EventHighway.EventHandlers)) |

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
