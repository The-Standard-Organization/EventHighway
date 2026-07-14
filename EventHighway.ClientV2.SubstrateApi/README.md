# EventHighway Client V2 — The SubstrateApi (Chat Sample)

The [`SubstrateApp`](../EventHighway.ClientV2.SubstrateApp/README.md) console sample runs the
whole NFlix media-catalogue story and prints the result. **This app is that same sample with
its walls taken down**: the hard-coded `CreateMediaItemViaExternalServiceAsync(...)` calls
become a public `POST /submit` endpoint you can reach from Postman, and the deliveries that
used to scroll past in a console arrive on a `POST /receive` endpoint and land on a chat UI
you can watch.

Everything else is unchanged. Same participants, same two addresses, same listeners, same
`MediaItemService` ingesting contributions into `NFlixMediaDB`, same substrate
(`EventHighwayDB`). Read the SubstrateApp README for how any of that works — this document
only covers what is new.

```
┌─ Postman ────┐
│ POST /submit ├──┐
└──────────────┘  │   ┌──────────────────────────────────────────────────────────┐
                  ├──▶│ /submit → ExternalMediaItemService                       │
┌─ the chat UI ┐  │   │        → [NFlix-ExternalContributions]                   │
│ Send button  ├──┘   │        → MediaItemService (saves to NFlixMediaDB)        │
└──────────────┘      │        → [NFlix-NewReleases]                             │
       ▲              │        → the unfiltered SubstrateApi listener            │
       │              │        → JoesRestApi delegate client                     │
       └──────────────┤        → POST /receive  ──▶ the chat UI                  │
                      └──────────────────────────────────────────────────────────┘
```

The send button takes the long way round on purpose. It does not call a service directly —
it POSTs to the app's own public `/submit`, exactly as an outside contributor would, and
then waits for the item to come back around as a delivery. **What you see on the UI is not
an echo of what you typed. It is the highway handing it back to you.**

---

## The one new listener

| | |
|---|---|
| **Address** | `NFlix-NewReleases` |
| **Filter** | none |
| **Promoted properties** | none |
| **Handler** | `SubstrateApi` (`3282e8fd-…`), a `JoesRestApiDelegateClient` pointed at `http://localhost:5150/receive` |
| **Participant** | `SubstrateApi` (`80aa28e0-…`), which also holds a secret — it publishes as well as listens |

Every other listener in the sample is picky about something (Joe wants movies rated 8.0+;
`MediaItemService` only listens on the intake address). This one is not: **anything that
reaches the address reaches the chat, whole**.

It is registered by **all three apps** — this one,
[`BasicApp`](../EventHighway.ClientV2.BasicApp/README.md) and
[`SubstrateApp`](../EventHighway.ClientV2.SubstrateApp/README.md) — under the same handler
Guid from the shared [`SeedIdentifiers`](../EventHighway.ClientV2.SubstrateApp/SeedIdentifiers.cs),
and all three point it at the same real localhost address. A listener row names a handler by
Guid; whichever process is dispatching resolves that Guid against its *own* in-memory handler
list. So whoever dispatches a release, the delivery lands on the one running chat UI.

Run any of them alongside this app and watch:

```
dotnet run --project EventHighway.ClientV2.SubstrateApi    # http://localhost:5150
dotnet run --project EventHighway.ClientV2.BasicApp        # its valid events appear on the UI
dotnet run --project EventHighway.ClientV2.SubstrateApp    # so do its accepted contributions
```

Invalid traffic does not appear, and that is the point of watching: BasicApp's
loop-quarantined Top Gun re-submissions and its unauthorised John Wick never reach the
address, so they never reach the chat.

---

## The two endpoints

### `POST /submit` — the public intake

**The UI shows you this call so you can make it yourself.** The panel at the top of the page
spells out the verb, the url, both credential headers *with their live values*, and a sample
body — each with a copy button, plus **Copy as cURL**, which Postman imports directly
(Import → Raw text). Every value comes from the app's own configuration through the same
broker the Send button uses, so what is on screen is what the app would actually send; a
documented example drifts, this cannot.

Credentials travel in **headers**, never in the body. A body is data a caller can pass
around; headers are how a caller identifies itself.

```http
POST http://localhost:5150/submit
Content-Type: application/json
X-EventHighwayParticipant: 80aa28e0-faca-4984-a1ac-bfa2e2d3926c
X-EventHighwayParticipantSecret: SubstrateApi

{
  "Id": "11111111-2222-3333-4444-555555555555",
  "Title": "Dune: Part Two",
  "Type": "Movie",
  "Genres": [ "Sci-Fi", "Adventure" ],
  "Rating": 8.5
}
```

`Rating` is accepted as a number (`8.5`) or as a string (`"8.5"`) — it rides the highway as
a string so it can be promoted and filtered on.

| Response | When |
|---|---|
| `202 Accepted` | The substrate took it. What this endpoint owns is a *submission*; the media item itself is created downstream, by whoever is listening. |
| `400 Bad Request` | Something the caller can fix, with the reason: `Event participant not found.`, `Event participant secret not found.`, `Event loop detected, event quarantined.`, or `... (Title: Text is required)`. |
| `500 Internal Server Error` | Anything else. |

The participant id and secret in the sample above are the ones in
[`appsettings.json`](appsettings.json) — the app's own identity, which the UI uses too. Any
seeded participant with a valid secret works: try NFlix's (`a817f520-…` / `NFlix`).

### `POST /receive` — where the highway knocks

Whatever is POSTed here is timestamped and shown on the UI, verbatim, pretty-printed if it
is JSON and raw if it is not. There is no schema: a delivery this app cannot parse is still
a delivery it should show. In practice the caller is always the `SubstrateApi` listener's
delegate client, in whichever app is dispatching.

### `GET /api/home` — heartbeat

Answers `The SubstrateApi is up and listening.`

---

## The chat

Two components, one view service. [`SubmitEndpoint`](Views/Components/SubmitEndpoint.razor)
publishes the call (above); [`EventChat`](Views/Components/EventChat.razor) shows what came
back. Both take a single dependency,
[`IEventChatsViewService`](Services/Views/EventChats/IEventChatsViewService.cs), which sits on
`MediaSubmissionService` — the one component that both *makes* the /submit call and can
*describe* it, from the same configuration. The log scrolls; the composer and the Send button
stay put.

Deliveries arrive on a **web request**, not on the component's circuit, so the store
([`ReceivedEventBroker`](Brokers/ReceivedEvents/ReceivedEventBroker.cs)) announces its own
changes and the component marshals the re-render back onto its own context. That is the whole
mechanism behind items appearing without a refresh. The log is in-memory and capped — the
chat is a live window onto the highway, not a second archive of it. EventHighway already
keeps the durable record of every delivery.

The composer re-arms with a **fresh `Id`** after each accepted send. Without that, pressing
Send twice would submit identical content, and the substrate's loop detection would quarantine
the second copy — a correct refusal, but a confusing one to demonstrate.

---

## Configuration

```json
{
  "JoesRestApi": {
    "Url": "http://localhost:9093/events",     // Joe's WireMock stand-in, as in the other samples
    "Secret": "joes-highway-secret"
  },

  "SubstrateApi": {
    "Url": "http://localhost:5150/receive",    // the delegate client's real target — not a stand-in
    "Secret": "substrate-api-secret",

    "SubmitUrl": "http://localhost:5150/submit",              // where the Send button posts
    "ParticipantId": "80aa28e0-faca-4984-a1ac-bfa2e2d3926c",  // the identity it posts under
    "ParticipantSecret": "SubstrateApi"
  }
}
```

Three ports matter and none of them collide, so all three samples can run at once:
SubstrateApp's WireMock on `9091`, BasicApp's on `9092`, this app's on `9093`, and this app
itself on `5150`.

`BasicApp` and `SubstrateApp` carry the same `SubstrateApi` section, minus the last three
keys — they deliver *to* the chat but never publish *as* it.

The app speaks plain HTTP with no redirect to https, deliberately: the delivery address is an
ordinary localhost url that three separate processes have to agree on, and https would only
give them a development certificate to argue about.

---

## Running it

```
dotnet run --project EventHighway.ClientV2.SubstrateApi
```

Then open <http://localhost:5150>. Both databases are created on first run if missing
(`EventHighwayDB` by the substrate, `NFlixMediaDB` by the app's storage broker — both on
`(localdb)\MSSQLLocalDB`), and the participants, addresses and listeners are seeded at
startup with the same fixed Guids the console samples use, so any of them can run first, in
any order, any number of times.

Press **Send**. The console shows the whole chain, inside one request:

```
Start processing HTTP request POST http://localhost:5150/submit
[SofaBox] New Release - Yellowstone (Series with rating of 8.6)
[Ann] New Release - Yellowstone (Series with rating of 8.6)
Received an event on /receive at 14:34:23 (127 characters).      ← the chat's own delivery
[FlakyBox] FAILED to deliver - Yellowstone (Series with rating of 8.6)
MediaItemService ingested Yellowstone (Series - 8.6 rating) and relayed MediaItemAdded
End processing HTTP request - 202
```

Joe is absent because Yellowstone is a `Series` and his filter wants movies. FlakyBox fails
because FlakyBox always fails. And `/receive` was called *before* `/submit` returned, because
an immediate event is dispatched inline — the delivery is nested inside the submission that
caused it.

That nesting is also why the app has a [`DatabaseGate`](Infrastructure/DatabaseGate.cs): a web
host serves requests in parallel, both EF contexts are single and shared, and the substrate
re-enters itself during dispatch. The gate serializes concurrent requests without deadlocking
on that re-entry — see the comments there, they are the interesting part.
