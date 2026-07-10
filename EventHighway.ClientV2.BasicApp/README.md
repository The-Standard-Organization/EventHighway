# EventHighway Client V2 — The NFlix Sample

This sample tells a small story to show how **EventHighway V2** routes events from a
publisher to interested subscribers. Everything in the story is wired up in
[`Program.cs`](Program.cs); this document explains *why* it is wired the way it is, and
then *how* you would do the same in your own application.

---

## 1. The Story

### The cast

**NFlix** is a streaming platform. Every time a new title becomes available it wants to
announce it once, to a single place, and let anyone who cares react however they like.
NFlix does not want to know who its audience is or what each member does with the
announcement — it just publishes "a new release is out".

To publish, NFlix is registered as a **participant** and is given a **secret**. Think of
the secret as NFlix's API key: every announcement NFlix sends is signed with its
participant id and secret so the system can prove the message genuinely came from NFlix.

NFlix announces its releases on a single channel — an **event address** called
**`NFlix-NewReleases`**. Everything flows through this one address.

### The subscribers

Three parties are interested in NFlix's releases, and their interests are different:

| Subscriber | Who they are | What they care about |
|---|---|---|
| **BingeBox** | A NFlix affiliate that re-lists everything | **Every** new release, no exceptions |
| **Joe** | An individual movie buff | **Only good movies** — a `Movie` (not a series) rated **8.0 or higher** |
| **Ann** | A **late joiner** | Every release — but she signs up *after* the originals were already sent, so she needs the back-catalogue too |

BingeBox and Joe are registered up front; Ann arrives later (see
[Section 1 — Ann, the late joiner](#ann-the-late-joiner)). Each is registered as a
**participant** and owns an **event listener** on the
`NFlix-NewReleases` address. A listener is a standing subscription: "when something lands
on this address, run my code."

- **BingeBox's listener has no filter.** It receives every release and reacts to all of them.
- **Joe's listener has a filter.** Before Joe's code is ever run, EventHighway checks the
  release against Joe's rule (`Type == "Movie"` **and** `Rating >= 8.0`). If the release
  does not match, Joe is quietly skipped — his code is never invoked for things he does
  not care about.

### What NFlix announces

NFlix publishes five titles. Each is a `MediaItem` (`Title`, `Type`, `Rating`, …):

| # | Title | Type | Rating | How it is sent |
|---|---|---|---|---|
| 1 | Yellowstone | Series | 8.6 | scheduled |
| 2 | Spider-Man: Across the Spider-Verse | Movie | 8.5 | immediate |
| 3 | Guardians of the Galaxy Vol. 3 | Movie | 7.9 | immediate |
| 4 | Top Gun: Maverick | Movie | 8.2 | scheduled, **sent 4 times** |
| 5 | John Wick: Chapter 4 | Movie | 7.6 | immediate, **sent with no participant id** |

A couple of these are deliberately mischievous so we can see the platform's safety nets:

- **Title 4 is announced four times.** A healthy publisher should not shout the same
  release over and over. EventHighway's **loop detection** is configured to allow only
  **one identical announcement per minute**, so the first Top Gun gets through and the
  next three are **quarantined** (blocked, never delivered).
- **Title 5 is sent without a participant id** (but with a random secret). This simulates
  an unauthenticated sender. EventHighway **rejects it** before it is ever stored.

### What each subscriber ends up seeing

| Release | BingeBox (everything) | Joe (good movies only) |
|---|---|---|
| Yellowstone (Series, 8.6) | ✅ delivered | ⏭️ skipped (not a Movie) |
| Spider-Verse (Movie, 8.5) | ✅ delivered | ✅ delivered |
| Guardians (Movie, 7.9) | ✅ delivered | ⏭️ skipped (rating < 8.0) |
| Top Gun #1 (Movie, 8.2) | ✅ delivered | ✅ delivered |
| Top Gun #2–4 | 🚫 loop-detected | 🚫 loop-detected |
| John Wick (no participant) | 🚫 unauthorised | 🚫 unauthorised |

So **BingeBox handles 4 releases** and **Joe handles 2** — exactly the two good movies.
BingeBox logs each delivery to the console; Joe's deliveries are **POSTed to his REST
API** (a WireMock stand-in started by the sample), so Joe produces no console lines of
his own — his outcomes appear in the per-subscriber summary as the API's recorded
response:

```
[BingeBox] New Release - Spider-Man: Across the Spider-Verse (Movie with rating of 8.5)
[BingeBox] New Release - Guardians of the Galaxy Vol. 3 (Movie with rating of 7.9)
...
  BingeBox: handled 4 event(s)
  Joe: handled 2 event(s)
    [Success] 200 OK Event received
```

### Ann, the late joiner

Ann shows up **after** the four releases have already gone out. Going forward she would
receive every new release automatically (her listener has no filter, just like
BingeBox) — but she has missed everything published before she signed up, and she does
not want a gap.

To bridge that gap we use a **targeted replay**: we re-deliver the already-published
releases, but **restricted to Ann's listener only**, so BingeBox and Joe are not
re-notified of things they already handled. Ann ends up with the same four releases
BingeBox got:

| Release | Ann (late joiner, back-filled by replay) |
|---|---|
| Yellowstone (Series, 8.6) | ✅ replayed |
| Spider-Verse (Movie, 8.5) | ✅ replayed |
| Guardians (Movie, 7.9) | ✅ replayed |
| Top Gun #1 (Movie, 8.2) | ✅ replayed |
| Top Gun #2–4 (loop-detected) | 🚫 quarantined — not replayed |

```
── Replaying archived releases to Ann ──
[Ann] New Release - Guardians of the Galaxy Vol. 3 (Movie with rating of 7.9)
[Ann] New Release - Yellowstone (Series with rating of 8.6)
...
  Ann: handled 4 event(s)
```

How this works in EventHighway is the subject of [Section 5](#5-late-joiners-targeted-replay).

### Joe asks for a re-run of one release

Separately, **Joe** reports that something went wrong on **his** side while processing one
specific release — *Spider-Man: Across the Spider-Verse* — and asks for that **one event**
to be sent to him again. This is the same targeted-replay machinery as Ann's back-fill, but
pointed at a **single, already-known event id** and delivered to **Joe's existing listener
only**. The sample does this right before the health summary, and Joe re-handles exactly
that one release:

```
── Replaying Spider-Verse to Joe ──
  Joe: handled 1 event(s)
    [Success] 200 OK Event received
```

---

## 2. How it is set up

The story above maps almost one-to-one onto the EventHighway V2 API. Here is each step.

### Step 1 — Create the client and configure loop detection

The client is created with a connection string and an optional configuration. Here we
tune loop detection so the story's "shout it four times" scenario is blocked after the
first announcement.

```csharp
var configuration = new EventHighwayConfiguration();
configuration.LoopDetection.Enabled = true;
configuration.LoopDetection.Threshold = 0;             // allow no repeats…
configuration.LoopDetection.Window = TimeSpan.FromMinutes(1);  // …within a one-minute window

var client = new EventHighwayClient(connectionString, configuration);
```

Loop detection fingerprints each event by address + name + a hash of its content. With
`Threshold = 0`, the first occurrence is allowed and any identical event seen again
within the window is **quarantined**.

### Step 2 — Write the subscribers' handlers

A handler is the code that runs when an event is delivered. The simplest kind is a
`DelegateEventHandler`: you give it a stable `Id`, a function, and (optionally) a **name**.

```csharp
var bingeBoxHandler = new DelegateEventHandler(
    Guid.NewGuid(),
    (content, cancellationToken) =>
    {
        MediaItem item = Deserialize(content);
        Console.WriteLine($"[BingeBox] New Release - {item.Title} ({item.Type} with rating of {item.Rating})");

        return ValueTask.FromResult(new EventHandlerResult
        {
            IsSuccess = true,
            Response = item.Title,
            ResponseCode = "200",
            ResponseMessage = "OK"
        });
    },
    name: "BingeBox");
```

> **The handler is the integration point.** In this sample the function just **writes to
> the console**, but that body is the place where your real business logic goes. It could
> just as easily **call a REST API** to forward the release to BingeBox's catalogue
> service, drop a message on a queue, write to a database, send an email, or run any other
> in-process logic. Whatever you return in the `EventHandlerResult` (`Response`,
> `ResponseCode`, `ResponseMessage`, `IsSuccess`) is stored so you can audit exactly what
> each subscriber did with each event.

Joe's handler shows the other end of that spectrum: instead of an inline lambda, the
function comes from a **packaged delegate client library**
([`EventHighway.EventHandlers.Delegates.JoesRestApi`](../EventHighway.EventHandlers.Delegates.JoesRestApi/README.md)).
Its exposed method matches the delegate signature exactly, so it is passed as a method
group — the registered function **is** the library's method, which POSTs the raw event
content to Joe's REST API using the url and `X-Highway` secret from `appsettings.json`
(the sample binds a WireMock stand-in to that configured url):

```csharp
var joesRestApiDelegateClient = new JoesRestApiDelegateClient(appSettings);

var joeHandler = new DelegateEventHandler(
    SeedIdentifiers.JoeHandler,
    joesRestApiDelegateClient.PostToJoesRestApiAsync,
    name: "Joe");
```

Both are then registered with the client:

```csharp
client.V2
    .RegisterEventHandler(bingeBoxHandler)
    .RegisterEventHandler(joeHandler);
```

> **Give each handler a distinct identity.** Handlers are resolved by their `Id`, which
> must be unique per registration. The optional `name` ("BingeBox", "Joe") makes logs and
> health output readable.

### Step 3 — Register the publisher (NFlix) and its secret

NFlix is a participant. Adding it returns the participant with its generated `Id`, which we
use to mint a secret. (The service assigns participant and secret ids, so you leave `Id`
unset.)

```csharp
EventParticipantV2 nflix =
    await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(new EventParticipantV2
    {
        Name = "NFlix",
        Description = "NFlix streaming platform.",
        IsActive = true,
        CreatedDate = now,
        UpdatedDate = now
    });

await client.V2.EventParticipantSecretV2Client.AddEventParticipantSecretV2Async(new EventParticipantSecretV2
{
    Secret = "NFlix",
    ParticipantId = nflix.Id,
    IsActive = true,
    CreatedDate = now,
    UpdatedDate = now
});
```

### Step 4 — Register (or add) the channel

The address is the single channel everything flows through. `RetrieveOrRegister` makes the
call idempotent.

```csharp
EventAddressV2 newReleases =
    await client.V2.EventAddressV2Client.RetrieveOrRegisterEventAddressV2Async(new EventAddressV2
    {
        Id = Guid.NewGuid(),
        Name = "NFlix-NewReleases",
        Description = "NFlix New Releases",
        CreatedDate = now,
        UpdatedDate = now
    });
```

### Step 5 — Subscribe BingeBox (everything)

BingeBox is a participant that owns a listener on the address. No filter means it receives
every release.

```csharp
EventParticipantV2 bingeBox = await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(/* … */);

await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "BingeBox New Releases Listener",
    HandlerId = bingeBoxHandler.Id,
    HandlerName = bingeBoxHandler.Name,
    EventAddressId = newReleases.Id,
    ParticipantId = bingeBox.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

### Step 6 — Subscribe Joe (good movies only)

Joe's listener adds two things on top of BingeBox's:

- **`PromotedProperties`** — a comma-separated list of fields to lift out of the event
  content so they can be tested (`"Title,Type,Rating"`).
- **`FilterCriteria`** — a boolean expression over those promoted properties. Joe is only
  invoked when it evaluates to `true`.

```csharp
await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Joe Good Movies Listener",
    HandlerId = joeHandler.Id,
    HandlerName = joeHandler.Name,
    EventAddressId = newReleases.Id,
    ParticipantId = joe.Id,
    PromotedProperties = "Title,Type,Rating",
    FilterCriteria = "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
    CreatedDate = now,
    UpdatedDate = now
});
```

`meta("…")` reads a promoted property by name. When Joe's filter does not match, the event
is recorded as `SkippedNotMatchingFilter` and Joe's handler is never called.

### Step 7 — NFlix publishes the releases

Each release is sent as an `EventV2`. The content is the JSON of a `MediaItem`, signed with
NFlix's participant id and secret. `ScheduledDate` decides *when* it runs:

- `null` (or a past time) → **immediate**: delivered as part of the submit call.
- a future time → **scheduled**: stored and delivered later when you fire pending events.

```csharp
var eventV2 = new EventV2
{
    Id = Guid.NewGuid(),
    Content = JsonSerializer.Serialize(item, MediaJsonOptions),
    EventName = item.Title,
    EventAddressId = newReleases.Id,
    ScheduledDate = scheduled ? DateTimeOffset.UtcNow.AddSeconds(1) : null,
    ParticipantId = nflix.Id,           // who is sending
    ParticipantSecret = "NFlix",        // proof it is really NFlix
    CreatedDate = now,
    UpdatedDate = now
};

await client.V2.EventV2Client.SubmitEventV2Async(eventV2);
```

This single call is where the safety nets fire:

- The **four Top Gun** submissions share identical content, so after the first one the rest
  trip loop detection and throw `LoopDetectedEventV2CoordinationException`.
- The **John Wick** submission has `ParticipantId = null` but a secret, which is rejected up
  front ("a participant secret requires a participant id").

The sample wraps each submit in a `try/catch` and prints whether it was accepted or blocked.

### Step 8 — Fire the scheduled releases

Immediate events have already been delivered. Scheduled ones (Yellowstone, the surviving
Top Gun) are waiting until their time passes; firing pending events delivers them.

```csharp
await client.V2.EventV2Client.FireScheduledPendingEventV2sAsync();
```

In a real service this runs on a timer or background job.

### Step 9 — Read what each subscriber did

Every delivery — success, skip, or error — is recorded as a `ListenerEventV2`. The sample
retrieves them and groups by subscriber to produce the final summary.

```csharp
IQueryable<ListenerEventV2> all =
    await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

foreach (ListenerEventV2 listenerEvent in all.Where(e => e.EventListenerId == bingeBoxListener.Id))
{
    // listenerEvent.Status, ResponseCode, ResponseMessage, Response
}
```

---

## 3. The content schema

The event payload is a plain object. NFlix and its subscribers simply agree on its shape:

```csharp
public class MediaItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }      // "Movie" or "Series"
    public List<string> Genres { get; set; } = new();
    public double Rating { get; set; }
}
```

> **A note on `Rating`.** Promoted properties are read out of the JSON as strings, so the
> sample serialises numbers as strings (`JsonNumberHandling.WriteAsString`) and reads them
> back as numbers (`AllowReadingFromString`). That is why Joe's filter parses the rating
> with `double.Parse(meta("Rating"))` before comparing it.

---

## 4. The actors at a glance

| Actor | EventHighway concept | Role in the story |
|---|---|---|
| NFlix | Participant + secret | Authenticated publisher of releases |
| `NFlix-NewReleases` | Event address | The single channel releases flow through |
| BingeBox | Participant + listener (no filter) | Wants every release |
| Joe | Participant + listener (promoted props + filter) | Wants only Movies rated ≥ 8.0 |
| Ann | Participant + listener (no filter), added late | A late joiner back-filled by replay |
| A release | Event (`EventV2`) | One announcement, immediate or scheduled |
| The handler delegate | `DelegateEventHandler` | The subscriber's reaction — console here, **your REST call / business logic in production** |
| Loop detection | Configuration | Blocks the same release announced repeatedly |
| Participant validation | Built-in | Blocks unauthenticated senders |
| Archive + targeted replay | Lifecycle | Moves handled events aside, then re-delivers them to a chosen listener |
| `ListenerEventV2` | Result record | The audit trail of who handled what |

---

## 5. Late joiners: targeted replay

Ann signs up after the four releases have already been published and handled. EventHighway
back-fills her using **archiving** plus a **targeted replay**.

### Step 1 — Archive the handled releases

Replay does not read from the live events table — it reads from the **archive**. So before
replaying we archive the processed events. `ArchiveEventV2sAsync` moves events aside once
they are settled:

- **successful** events,
- **dead** events that have exhausted their retries (events still holding retries are left
  alone so they can be re-attempted),
- **quarantined** (loop-detected) events.

```csharp
await client.V2.ArchivingEventV2Client.ArchiveEventV2sAsync();
```

### Step 2 — Add the late joiner

Ann is just another participant with a no-filter listener — added now, after the originals
were sent.

```csharp
EventParticipantV2 ann = await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(/* … */);

var annListener = await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Ann New Releases Listener",
    HandlerId = annHandler.Id,
    HandlerName = annHandler.Name,
    EventAddressId = newReleases.Id,
    ParticipantId = ann.Id,
    CreatedDate = lateNow,
    UpdatedDate = lateNow
});
```

### Step 3 — Replay the archived releases to Ann only

The targeted overload of `ReplayEventArchiveV2sAsync` takes a **single archived event id**
and the **listener ids** to deliver it to. We call it for each release Ann missed,
restricting delivery to her listener so BingeBox and Joe are never re-notified.

```csharp
foreach (Guid eventId in acceptedEventIds)
{
    await client.V2.ReplayingEventV2Client.ReplayEventArchiveV2sAsync(
        eventV2Id: eventId,
        eventAddressId: newReleases.Id,
        eventListenerIds: new[] { annListener.Id },
        allowReplayOfQuarantinedItem: false);
}

// Replay restores the events and queues replay listener events; this delivers them.
await client.V2.ReplayingEventV2Client.ProcessReplayedListenerEventV2sAsync();
```

What the targeted replay does for each event:

1. Looks the event up **in the archive** by id (nothing happens if it is not archived).
2. Honours the **quarantine gate** — a loop-detected event is skipped unless
   `allowReplayOfQuarantinedItem: true`. (Because the replay path does **not** re-run loop
   detection, a quarantined event allowed back in will succeed the second time.)
3. Restores the event and creates fresh **replay listener events for the supplied listeners
   only** — so Ann is hydrated even though she never had a listener event for these
   releases, and no one else is touched.

`ProcessReplayedListenerEventV2sAsync` then runs Ann's handler for each, exactly as if the
releases had just arrived. Ann ends up having handled all four.

### Step 4 — Housekeeping

A final `ArchiveEventV2sAsync` tidies the events that replay restored, and the run ends with
the health summary.

```csharp
await client.V2.ArchivingEventV2Client.ArchiveEventV2sAsync();
await PrintHealthSummaryAsync(client);
```

> **Why replay sources from the archive.** Keeping the live events table lean is the whole
> point of archiving; replay deliberately reads the archive so that re-delivery and
> back-fill work against the durable record rather than the hot path. That is why the
> sample archives *before* replaying Ann's back-catalogue.

### Re-processing a single event for an existing listener (Joe)

The same targeted replay also covers a very different need: **re-running one specific event
for a listener that already exists**. Joe reports a processing problem on his side with one
release and asks for just that event again.

To replay a *known* event you need its id. The sample mints each event id up front and
passes it in, so the id is captured at submit time:

```csharp
private static async Task<Guid?> SubmitMediaAsync(
    Guid eventV2Id,              // supplied by the caller, not generated inside
    EventHighwayClient client,
    Guid eventAddressId,
    MediaItem item,
    bool scheduled,
    Guid? participantId,
    string secret,
    int attempt = 0)
{
    var eventV2 = new EventV2 { Id = eventV2Id, /* … */ };
    // …
}
```

```csharp
Guid spiderVerseEventId = Guid.NewGuid();
await SubmitMediaAsync(spiderVerseEventId, client, newReleases.Id, spiderVerse, /* … */);
```

Then, after the housekeeping archive, that single event is replayed to **Joe's listener
only**:

```csharp
await client.V2.ReplayingEventV2Client.ReplayEventArchiveV2sAsync(
    eventV2Id: spiderVerseEventId,
    eventAddressId: newReleases.Id,
    eventListenerIds: new[] { joeListener.Id },
    allowReplayOfQuarantinedItem: false);

await client.V2.ReplayingEventV2Client.ProcessReplayedListenerEventV2sAsync();
```

Joe re-handles that one release and nothing else — BingeBox and Ann are untouched, and no
other events are re-sent.
