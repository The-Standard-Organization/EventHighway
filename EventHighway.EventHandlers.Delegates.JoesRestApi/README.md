# EventHighway.EventHandlers.Delegates.JoesRestApi

A sample **delegate client** library: a packaged integration whose exposed method is
*delegate-compatible* with `DelegateEventHandler`, so consuming apps can plug it in
without writing any handler code of their own.

## The pattern

`EventHighway.EventHandlers` (closed for extension) ships `DelegateEventHandler`, which
adapts a `Func<string, CancellationToken, ValueTask<EventHandlerResult>>` into an
`IEventHandler`. A *delegate client* library like this one supplies that function as a
first-class, Standard-shaped component:

```
EventHighway.EventHandlers.Delegates.<Integration>     ← one library per downstream
└── Clients\<Integration>DelegateClient                ← exposes the delegate-compatible method
    └── Services\Foundations\EventPosts\EventPostService   ← validations + broker calls
        ├── Brokers\Configurations\ConfigurationBroker     ← reads the appsettings section
        └── Brokers\Apis\ApiBroker                         ← POSTs the event content
```

The exposed method never throws: validation problems map to a `400`-shaped
`EventHandlerResult`, delivery problems to a `502`, anything else to a `500`.

## Configuration

The client reads the `JoesRestApi` section of the host's configuration:

```json
{
  "JoesRestApi": {
    "Url": "http://localhost:9091/events",
    "Secret": "joes-highway-secret"
  }
}
```

Each event's content is POSTed to `Url` as `application/json` with the secret in an
`X-Highway` request header.

## Wiring it up

With dependency injection:

```csharp
services.AddJoesRestApiDelegateClient(configuration);

// at registration time
IEventHandler joeHandler = new DelegateEventHandler(
    SeedIdentifiers.JoeHandler,
    joesRestApiDelegateClient.PostToJoesRestApiAsync,
    name: "Joe");
```

Without a container, construct it directly:

```csharp
var joesRestApiDelegateClient = new JoesRestApiDelegateClient(configuration);
```

The identity (`Guid` and handler name) deliberately stays with the consuming app — it
identifies a *registration* on the highway; this library only knows *how to deliver*.
