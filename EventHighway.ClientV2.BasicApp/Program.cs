// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.Seed;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.EventHandlers;
using EventHighway.SqlServer.Brokers;

public partial class Program
{
    // Rating is written as a JSON string so it can be used as a promoted property
    // (promotion reads JSON values as strings) and read back into a double by handlers.
    private static readonly JsonSerializerOptions MediaJsonOptions = new()
    {
        NumberHandling =
            JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
    };

    private static async Task Main(string[] args)
    {
        string connectionString = string.Concat(
            "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;",
            "Trusted_Connection=True;MultipleActiveResultSets=true");

        SqlServerStorageBrokerProvider storageProvider =
            new SqlServerStorageBrokerProvider(connectionString);

        // =========================================================
        // 1) Configure loop detection: only allow 1 identical item per minute
        // =========================================================
        var configuration = new EventHighwayConfiguration();
        configuration.LoopDetection.Enabled = true;
        configuration.LoopDetection.Threshold = 0;
        configuration.LoopDetection.Window = TimeSpan.FromMinutes(1);

        var client = new EventHighwayClient(storageProvider, configuration);

        // =========================================================
        // 2) Create and register the handlers
        // =========================================================
        var bingeBoxHandler = new DelegateEventHandler(
            SeedIdentifiers.BingeBoxHandler,
            (content, cancellationToken) =>
            {
                MediaItem item = Deserialize(content);

                Console.WriteLine(
                    $"[BingeBox] New Release - {item.Title} " +
                    $"({item.Type} with rating of {item.Rating})");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = item.Title,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            },
            name: "BingeBox");

        var joeHandler = new DelegateEventHandler(
            SeedIdentifiers.JoeHandler,
            (content, cancellationToken) =>
            {
                MediaItem item = Deserialize(content);

                Console.WriteLine(
                    $"[Joe] New Release - {item.Title} " +
                    $"({item.Type} with rating of {item.Rating})");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = item.Title,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            },
            name: "Joe");

        var annHandler = new DelegateEventHandler(
            SeedIdentifiers.AnnHandler,
            (content, cancellationToken) =>
            {
                MediaItem item = Deserialize(content);

                Console.WriteLine(
                    $"[Ann] New Release - {item.Title} " +
                    $"({item.Type} with rating of {item.Rating})");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = item.Title,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            },
            name: "Ann");

        client.V2
            .RegisterEventHandler(bingeBoxHandler)
            .RegisterEventHandler(joeHandler)
            .RegisterEventHandler(annHandler);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        // Idempotent on the (fixed) Id so re-running this app — or sharing the database with the
        // SubstrateApp, which uses the same seed identifiers — reuses the existing rows.
        async Task<EventParticipantV2> GetOrAddParticipantAsync(EventParticipantV2 participant)
        {
            IEnumerable<EventParticipantV2> existingParticipants =
                await client.V2.EventParticipantV2Client.RetrieveAllEventParticipantV2sAsync();

            return existingParticipants.FirstOrDefault(existing => existing.Id == participant.Id)
                ?? await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(participant);
        }

        async Task GetOrAddSecretAsync(EventParticipantSecretV2 secret)
        {
            IEnumerable<EventParticipantSecretV2> existingSecrets =
                await client.V2.EventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync();

            if (existingSecrets.All(existing => existing.Id != secret.Id))
            {
                await client.V2.EventParticipantSecretV2Client.AddEventParticipantSecretV2Async(secret);
            }
        }

        // =========================================================
        // 3) Register the publishing participant (NFlix) and its secret
        // =========================================================
        EventParticipantV2 nflix =
            await GetOrAddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.NFlixParticipant,
                    Name = "NFlix",
                    Description = "NFlix streaming platform.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        await GetOrAddSecretAsync(
            new EventParticipantSecretV2
            {
                Id = SeedIdentifiers.NFlixSecret,
                Secret = "NFlix",
                EventParticipantV2Id = nflix.Id,
                IsActive = true,
                CreatedDate = now,
                UpdatedDate = now
            });

        // =========================================================
        // 4) Register (or add) the event address
        // =========================================================
        EventAddressV2 newReleases =
            await client.V2.EventAddressV2Client.RetrieveOrRegisterEventAddressV2Async(
                new EventAddressV2
                {
                    Id = SeedIdentifiers.NFlixNewReleasesAddress,
                    Name = "NFlix-NewReleases",
                    Description = "NFlix New Releases",
                    CreatedDate = now,
                    UpdatedDate = now
                });

        // =========================================================
        // 5) BingeBox participant + listener (receives every release)
        // =========================================================
        EventParticipantV2 bingeBox =
            await GetOrAddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.BingeBoxParticipant,
                    Name = "BingeBox",
                    Description = "BingeBox a NFlix affiliate",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        var bingeBoxListener =
            await client.V2.EventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.BingeBoxNewReleasesListener,
                    Name = "BingeBox New Releases Listener",
                    Description = "Receives every NFlix new release.",
                    HandlerId = bingeBoxHandler.Id,
                    HandlerName = bingeBoxHandler.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = bingeBox.Id,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        // =========================================================
        // 6) Joe participant + listener (only good movies)
        // =========================================================
        EventParticipantV2 joe =
            await GetOrAddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.JoeParticipant,
                    Name = "Joe",
                    Description = "Joe, a movie buff.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        var joeListener =
            await client.V2.EventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.JoeGoodMoviesListener,
                    Name = "Joe Good Movies Listener",
                    Description = "Receives movies rated 8.0 or higher.",
                    HandlerId = joeHandler.Id,
                    HandlerName = joeHandler.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = joe.Id,
                    PromotedProperties = "Title,Type,Rating",
                    FilterCriteria =
                        "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
                    CreatedDate = now,
                    UpdatedDate = now
                });

        // =========================================================
        // 7) Submit events as NFlix (with participant id + secret)
        // =========================================================
        Console.WriteLine("\n── Submitting events ──");

        var yellowstone = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Yellowstone",
            Type = "Series",
            Rating = 8.6
        };

        var spiderVerse = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Spider-Man: Across the Spider-Verse",
            Type = "Movie",
            Rating = 8.5
        };

        var guardians = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Guardians of the Galaxy Vol. 3",
            Type = "Movie",
            Rating = 7.9
        };

        var topGun = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Top Gun: Maverick",
            Type = "Movie",
            Rating = 8.2
        };

        var acceptedEventIds = new List<Guid>();

        // We mint each event id up front so we can track and later replay a specific one.
        Guid spiderVerseEventId = Guid.NewGuid();

        // 1) Yellowstone — scheduled
        AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(Guid.NewGuid(), client, newReleases.Id, yellowstone,
            scheduled: true, participantId: nflix.Id, secret: "NFlix"));

        // 2) Spider-Verse — immediate
        AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(spiderVerseEventId, client, newReleases.Id, spiderVerse,
            scheduled: false, participantId: nflix.Id, secret: "NFlix"));

        // 3) Guardians — immediate
        AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(Guid.NewGuid(), client, newReleases.Id, guardians,
            scheduled: false, participantId: nflix.Id, secret: "NFlix"));

        // 4) Top Gun — scheduled, submitted 4 times to simulate a loop
        for (int attempt = 1; attempt <= 4; attempt++)
        {
            AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(Guid.NewGuid(), client, newReleases.Id, topGun,
                scheduled: true, participantId: nflix.Id, secret: "NFlix",
                attempt: attempt));
        }

        // 5) John Wick — unauthorised: null participant id with a random secret
        var johnWick = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "John Wick: Chapter 4",
            Type = "Movie",
            Rating = 7.6
        };

        await SubmitMediaAsync(Guid.NewGuid(), client, newReleases.Id, johnWick,
            scheduled: false, participantId: null, secret: Guid.NewGuid().ToString());

        // =========================================================
        // 8) Fire the scheduled (pending) events
        // =========================================================
        Console.WriteLine("\n── Firing scheduled events ──");
        await Task.Delay(TimeSpan.FromSeconds(3));
        await client.V2.EventV2Client.FireScheduledPendingEventV2sAsync();

        // =========================================================
        // 9) Summary of what the original subscribers recorded
        // =========================================================
        await PrintListenerSummaryAsync(
            client,
            (bingeBoxListener.Id, "BingeBox"),
            (joeListener.Id, "Joe"));

        // =========================================================
        // 10) Ann joins late and back-fills via a targeted replay
        // =========================================================
        // Replay sources events from the archive, so first archive the processed
        // events (successful + quarantined) to make them available to replay.
        await client.V2.ArchivingEventV2Client.ArchiveEventV2sAsync();

        DateTimeOffset lateNow = DateTimeOffset.UtcNow;

        EventParticipantV2 ann =
            await GetOrAddParticipantAsync(
                new EventParticipantV2
                {
                    Id = SeedIdentifiers.AnnParticipant,
                    Name = "Ann",
                    Description = "Ann",
                    IsActive = true,
                    CreatedDate = lateNow,
                    UpdatedDate = lateNow
                });

        var annListener =
            await client.V2.EventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(
                new EventListenerV2
                {
                    Id = SeedIdentifiers.AnnNewReleasesListener,
                    Name = "Ann New Releases Listener",
                    Description = "Ann, a late joiner who wants the back-catalogue.",
                    HandlerId = annHandler.Id,
                    HandlerName = annHandler.Name,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = ann.Id,
                    CreatedDate = lateNow,
                    UpdatedDate = lateNow
                });

        Console.WriteLine("\n── Replaying archived releases to Ann ──");

        // Targeted replay: re-deliver each archived release to Ann's listener only.
        // Quarantined (loop-detected) events are skipped unless explicitly allowed.
        foreach (Guid eventId in acceptedEventIds)
        {
            await client.V2.ReplayingEventV2Client.ReplayEventArchiveV2sAsync(
                eventV2Id: eventId,
                eventAddressId: newReleases.Id,
                eventListenerIds: new[] { annListener.Id },
                allowReplayOfQuarantinedItem: false);
        }

        await client.V2.ReplayingEventV2Client.ProcessReplayedListenerEventV2sAsync();

        await PrintListenerSummaryAsync(client, (annListener.Id, "Ann"));

        // =========================================================
        // 11) Archive again (housekeeping)
        // =========================================================
        await client.V2.ArchivingEventV2Client.ArchiveEventV2sAsync();

        // =========================================================
        // 12) Joe asks to re-process one specific release he had trouble with
        // =========================================================
        Console.WriteLine("\n── Replaying Spider-Verse to Joe ──");

        await client.V2.ReplayingEventV2Client.ReplayEventArchiveV2sAsync(
            eventV2Id: spiderVerseEventId,
            eventAddressId: newReleases.Id,
            eventListenerIds: new[] { joeListener.Id },
            allowReplayOfQuarantinedItem: false);

        await client.V2.ReplayingEventV2Client.ProcessReplayedListenerEventV2sAsync();

        await PrintListenerSummaryAsync(client, (joeListener.Id, "Joe"));

        // =========================================================
        // 13) Health summary
        // =========================================================
        await PrintHealthSummaryAsync(client);
    }

    private static async Task<Guid?> SubmitMediaAsync(
        Guid eventV2Id,
        EventHighwayClient client,
        Guid eventAddressId,
        MediaItem item,
        bool scheduled,
        Guid? participantId,
        string secret,
        int attempt = 0)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var eventV2 = new EventV2
        {
            Id = eventV2Id,
            Content = JsonSerializer.Serialize(item, MediaJsonOptions),
            EventName = item.Title,
            EventAddressV2Id = eventAddressId,
            ScheduledDate = scheduled ? now.AddSeconds(1) : null,
            EventParticipantV2Id = participantId,
            EventParticipantV2Secret = secret,
            CreatedDate = now,
            UpdatedDate = now
        };

        string label = attempt > 0 ? $"{item.Title} (attempt {attempt})" : item.Title;
        string kind = scheduled ? "scheduled" : "immediate";

        try
        {
            await client.V2.EventV2Client.SubmitEventV2Async(eventV2);

            WriteMarker(
                "  [Success]", ConsoleColor.Green,
                $" accepted  {label} [{kind}]");

            return eventV2.Id;
        }
        catch (Exception exception)
        {
            WriteMarker(
                "  [Fail]   ", ConsoleColor.Red,
                $" blocked   {label} [{kind}] - {RootMessage(exception)}");

            return null;
        }
    }

    private static void AddIfAccepted(List<Guid> acceptedEventIds, Guid? eventId)
    {
        if (eventId.HasValue)
            acceptedEventIds.Add(eventId.Value);
    }

    private static void WriteMarker(string marker, ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.Write(marker);
        Console.ResetColor();
        Console.WriteLine(text);
    }

    private static async Task PrintListenerSummaryAsync(
        EventHighwayClient client,
        params (Guid ListenerId, string Participant)[] listeners)
    {
        IQueryable<ListenerEventV2> all =
            await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

        Console.WriteLine("\n── Listener results ──");

        foreach ((Guid listenerId, string participant) in listeners)
        {
            List<ListenerEventV2> events =
                all.Where(listenerEvent => listenerEvent.EventListenerV2Id == listenerId)
                    .ToList();

            int handled = events.Count(listenerEvent =>
                listenerEvent.ResponseCode == "200");

            Console.WriteLine($"\n  {participant}: handled {handled} event(s)");

            foreach (ListenerEventV2 listenerEvent in events)
            {
                ConsoleColor color = listenerEvent.Status switch
                {
                    ListenerEventStatusV2.Success => ConsoleColor.Green,
                    ListenerEventStatusV2.Error => ConsoleColor.Red,
                    ListenerEventStatusV2.Pending => ConsoleColor.Yellow,
                    _ => ConsoleColor.Gray,
                };

                WriteMarker(
                    $"    [{listenerEvent.Status}]", color,
                    $" {listenerEvent.ResponseCode} " +
                    $"{listenerEvent.ResponseMessage} {listenerEvent.Response}");
            }
        }

        Console.WriteLine();
    }

    private static async Task PrintHealthSummaryAsync(EventHighwayClient client)
    {
        IEnumerable<HealthCheckItemV2> summary =
            await client.V2.HealthStatusClientV2.RetrieveHealthRagStatusV2Async();

        Console.WriteLine("── Health summary ──");

        string? currentGrouping = null;

        foreach (HealthCheckItemV2 item in summary)
        {
            if (item.Grouping != currentGrouping)
            {
                currentGrouping = item.Grouping;
                Console.WriteLine($"\n  {currentGrouping}");
            }

            ConsoleColor color = item.Status switch
            {
                nameof(HealthStatusV2.Green) => ConsoleColor.Green,
                nameof(HealthStatusV2.Amber) => ConsoleColor.Yellow,
                nameof(HealthStatusV2.Red) => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };

            WriteMarker(
                $"    [{item.Status,-5}]", color,
                $" {item.Item}: {item.Value}");
        }

        Console.WriteLine();
    }

    private static MediaItem Deserialize(string content) =>
        JsonSerializer.Deserialize<MediaItem>(content, MediaJsonOptions)
            ?? new MediaItem();

    private static string RootMessage(Exception exception)
    {
        Exception current = exception;

        while (current.InnerException is not null)
            current = current.InnerException;

        return current.Message;
    }
}

public class MediaItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Movie" or "Series"
    public List<string> Genres { get; set; } = new();
    public double Rating { get; set; }
}
