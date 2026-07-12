// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

// Reusable database hydrator for the EventHighway sample data. Inserts a year's worth of
// backdated EventV2 + ListenerEventV2 rows (and their EventArchiveV2 + ListenerEventArchiveV2
// counterparts) directly through the internal StorageBroker, bypassing foundation validation
// (which forbids a backdated CreatedDate). Any console app that references this project can call
// DatabaseHydrator.HydrateNewReleasesAsync(connectionString) to top up the database. Self-sufficient:
// the Ensure* helpers create the NFlix participant, the NFlix-NewReleases address and its listeners
// when missing (through the EventHighway client's RetrieveOrAdd/RetrieveOrRegister operations where
// available), so it runs against an empty database on its own; when the ClientV2.SubstrateApp/
// BasicApp samples have already run it reconciles to their rows via the shared well-known Guids
// instead of duplicating them. Re-running is safe: it only appends more traffic (fresh row Ids,
// backdated across the trailing year) and never mutates the existing config rows.

using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.SqlServer;

namespace EventHighway.Portal.Seed
{
    public static class DatabaseHydrator
    {
        private const string NewReleasesAddressName = "NFlix-NewReleases";
        private const string NFlixParticipantName = "NFlix";
        private const string NewReleaseEventName = "AddNewRelease";

        // Well-known identifiers shared with EventHighway.ClientV2.SubstrateApp's SeedIdentifiers.
        // Re-using the SAME Guids means this hydrator and that sample reconcile to the same rows
        // instead of creating duplicates — but without taking a dependency on the other console app.
        private static readonly Guid NFlixParticipantId =
            new Guid("a817f520-c7e5-4831-a67b-171902bf28ba");

        private static readonly Guid NewReleasesAddressId =
            new Guid("be0dd6e0-b545-435d-9541-d1ac386469ce");

        // SofaBox / Joe / Ann participants + listeners — the same set EventHighway.ClientV2.BasicApp
        // registers (same Guids as the shared SeedIdentifiers), so runs reconcile to the same rows.
        // Handler Ids are fixed here (BasicApp mints random ones at runtime) so the seeded listener
        // rows are stable across re-runs; the handlers themselves are in-memory dispatch objects.
        private static readonly ListenerSpec[] ListenerSpecs =
        {
            new ListenerSpec(
                ParticipantId: new Guid("72edb46a-4e55-49dc-8b92-16baf040c6fd"),
                ParticipantName: "SofaBox",
                ParticipantDescription: "SofaBox a NFlix affiliate",
                ListenerId: new Guid("07864612-508c-4177-a0b6-061f9efa48d8"),
                ListenerName: "SofaBox New Releases Listener",
                ListenerDescription: "Receives every NFlix new release.",
                HandlerId: new Guid("6326cae3-04ff-411f-93fb-e606859390f6"),
                HandlerName: "SofaBox",
                PromotedProperties: null,
                FilterCriteria: null),
            new ListenerSpec(
                ParticipantId: new Guid("523a9adc-a582-42da-ab0d-762eb8782962"),
                ParticipantName: "Joe",
                ParticipantDescription: "Joe, a movie buff.",
                ListenerId: new Guid("523a9adc-a582-42da-ab0d-762eb8782962"),
                ListenerName: "Joe Good Movies Listener",
                ListenerDescription: "Receives movies rated 8.0 or higher.",
                HandlerId: new Guid("9846c9e3-2843-4a2e-a586-4321c3a5f1a9"),
                HandlerName: "Joe",
                PromotedProperties: "Title,Type,Rating",
                FilterCriteria:
                    "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0"),
            new ListenerSpec(
                ParticipantId: new Guid("ab496d88-7cf5-4e8f-af45-5e75583fb5d0"),
                ParticipantName: "Ann",
                ParticipantDescription: "Ann",
                ListenerId: new Guid("ab496d88-7cf5-4e8f-af45-5e75583fb5d0"),
                ListenerName: "Ann New Releases Listener",
                ListenerDescription: "Ann, a late joiner who wants the back-catalogue.",
                HandlerId: new Guid("a9079276-fbbe-4176-9744-9fee3354f3e7"),
                HandlerName: "Ann",
                PromotedProperties: null,
                FilterCriteria: null),
        };

        // Volume targets per run (appended on top of whatever already exists), spread across the
        // trailing year. Live events feed the live dashboard groups; archived events feed the
        // Event Archives / Archived Listeners groups and the archived traffic series.
        private const int PastDays = 365;
        private const int LiveEventCount = 1825;      // ~5/day across the year
        private const int ArchivedEventCount = 730;   // ~2/day across the year

        // Outcome mix PER EVENT: ~80% fully-successful deliveries; the remainder is partial
        // successes (1-2 listeners errored, the rest succeeded), complete failures, and
        // loop-quarantined events (caught before dispatch, so no listener events). Errored
        // deliveries carry the dead/critical/healthy retry split (see RandomErrorRemaining).
        private const int AllSuccessPercent = 80;
        private const int PartialSuccessPercent = 8;
        private const int AllFailurePercent = 6;    // the remaining ~6% is quarantined

        // A slice of events draw their content hash from a shared per-run pool so duplicate
        // detection has repeated hashes to find.
        private const double DuplicateRate = 0.10;
        private const int DuplicateHashPoolSize = 12;
        private const int RetryAttemptsAllowed = 5;

        public static async Task HydrateNewReleasesAsync(string connectionString)
        {
            var broker = new StorageBroker(new SqlServerStorageBrokerProvider(connectionString));

            var client =
                new EventHighwayClient(new SqlServerStorageBrokerProvider(connectionString));

            DateTimeOffset now = DateTimeOffset.UtcNow;

            EventParticipantV2 nflix = await EnsureParticipantAsync(client, now);
            EventAddressV2 newReleases = await EnsureAddressAsync(client, now);

            List<EventListenerV2> addressListeners =
                await EnsureListenersAsync(client, newReleases.Id, now);

            var rng = new Random();

            // Repeated hashes => duplicate detection has something to find.
            string[] duplicateHashPool = Enumerable.Range(0, DuplicateHashPoolSize)
                .Select(_ => "DUP-" + Guid.NewGuid().ToString("N")[..12])
                .ToArray();

            string NextContentHash() =>
                rng.NextDouble() < DuplicateRate
                    ? duplicateHashPool[rng.Next(duplicateHashPool.Length)]
                    : "HASH-" + Guid.NewGuid().ToString("N")[..12];

            // ---- Live events + listener events across the trailing year ----
            var events = new List<EventV2>();
            var listenerEvents = new List<ListenerEventV2>();

            for (int index = 0; index < LiveEventCount; index++)
            {
                DateTimeOffset created = RandomPastMoment(rng, now);
                bool isScheduled = rng.Next(0, 2) == 0;
                EventOutcome outcome = RollOutcome(rng);

                var eventV2 = new EventV2
                {
                    Id = Guid.NewGuid(),
                    Content = BuildContent(rng),
                    EventName = NewReleaseEventName,
                    ContentHash = NextContentHash(),
                    Type = isScheduled ? EventTypeV2.Scheduled : EventTypeV2.Immediate,

                    Status = outcome == EventOutcome.Quarantined
                        ? EventStatusV2.Quarantined
                        : EventStatusV2.Active,

                    ScheduledDate = isScheduled ? created : (DateTimeOffset?)null,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = nflix.Id,
                    EventParticipantV2Secret = NFlixParticipantName,
                    CreatedDate = created,
                    UpdatedDate = created
                };

                events.Add(eventV2);

                // Quarantined events are caught by loop detection before dispatch.
                if (outcome == EventOutcome.Quarantined)
                {
                    continue;
                }

                HashSet<int> errorIndexes =
                    PickErrorIndexes(rng, addressListeners.Count, outcome);

                for (int listenerIndex = 0; listenerIndex < addressListeners.Count; listenerIndex++)
                {
                    EventListenerV2 listener = addressListeners[listenerIndex];
                    bool isSuccess = errorIndexes.Contains(listenerIndex) is false;
                    DateTimeOffset dispatched = created.AddSeconds(3);
                    int remainingRetries = isSuccess ? RetryAttemptsAllowed : RandomErrorRemaining(rng);

                    listenerEvents.Add(new ListenerEventV2
                    {
                        Id = Guid.NewGuid(),
                        Status = isSuccess ? ListenerEventStatusV2.Success : ListenerEventStatusV2.Error,
                        Response = isSuccess ? "Event received" : "Handler failed",
                        ResponseCode = isSuccess ? "200" : "503",
                        ResponseMessage = isSuccess ? "OK" : "Service Unavailable",
                        RemainingRetryAttempts = remainingRetries,
                        RetryAttemptsAllowed = RetryAttemptsAllowed,

                        NextRetryAttemptNotBefore = isSuccess is false && remainingRetries > 0
                            ? dispatched.AddMinutes(rng.Next(5, 240))
                            : (DateTimeOffset?)null,

                        DispatchedDate = dispatched,
                        EventV2Id = eventV2.Id,
                        EventAddressV2Id = eventV2.EventAddressV2Id,
                        EventListenerV2Id = listener.Id,
                        EventParticipantV2Id = listener.EventParticipantV2Id,
                        CreatedDate = dispatched,
                        UpdatedDate = dispatched
                    });
                }
            }

            // ---- Archived events + archived listener events across the trailing year ----
            var eventArchives = new List<EventArchiveV2>();
            var listenerEventArchives = new List<ListenerEventArchiveV2>();

            for (int index = 0; index < ArchivedEventCount; index++)
            {
                DateTimeOffset archived = RandomPastMoment(rng, now);
                DateTimeOffset created = archived.AddDays(-rng.Next(0, 7)).AddHours(-rng.Next(0, 24));
                bool isScheduled = rng.Next(0, 2) == 0;
                EventOutcome outcome = RollOutcome(rng);

                var eventArchive = new EventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Content = BuildContent(rng),
                    EventName = NewReleaseEventName,
                    ContentHash = NextContentHash(),
                    Type = isScheduled ? EventArchiveTypeV2.Scheduled : EventArchiveTypeV2.Immediate,

                    Status = outcome == EventOutcome.Quarantined
                        ? EventArchiveStatusV2.Quarantined
                        : EventArchiveStatusV2.Active,

                    ScheduledDate = isScheduled ? created : (DateTimeOffset?)null,
                    EventAddressV2Id = newReleases.Id,
                    EventParticipantV2Id = nflix.Id,
                    CreatedDate = created,
                    UpdatedDate = created,
                    ArchivedDate = archived
                };

                eventArchives.Add(eventArchive);

                // Quarantined events were caught by loop detection before dispatch.
                if (outcome == EventOutcome.Quarantined)
                {
                    continue;
                }

                HashSet<int> errorIndexes =
                    PickErrorIndexes(rng, addressListeners.Count, outcome);

                for (int listenerIndex = 0; listenerIndex < addressListeners.Count; listenerIndex++)
                {
                    EventListenerV2 listener = addressListeners[listenerIndex];
                    bool isSuccess = errorIndexes.Contains(listenerIndex) is false;
                    DateTimeOffset dispatched = created.AddSeconds(3);
                    int remainingRetries = isSuccess ? RetryAttemptsAllowed : RandomErrorRemaining(rng);

                    listenerEventArchives.Add(new ListenerEventArchiveV2
                    {
                        Id = Guid.NewGuid(),
                        Status = isSuccess
                            ? ListenerEventArchiveStatusV2.Success
                            : ListenerEventArchiveStatusV2.Error,
                        Response = isSuccess ? "Event received" : "Handler failed",
                        ResponseCode = isSuccess ? "200" : "503",
                        ResponseMessage = isSuccess ? "OK" : "Service Unavailable",
                        RemainingRetryAttempts = remainingRetries,
                        RetryAttemptsAllowed = RetryAttemptsAllowed,

                        NextRetryAttemptNotBefore = isSuccess is false && remainingRetries > 0
                            ? dispatched.AddMinutes(rng.Next(5, 240))
                            : (DateTimeOffset?)null,

                        DispatchedDate = dispatched,
                        EventV2Id = Guid.NewGuid(),
                        EventAddressV2Id = newReleases.Id,
                        EventListenerV2Id = listener.Id,
                        EventArchiveV2Id = eventArchive.Id,
                        EventParticipantV2Id = listener.EventParticipantV2Id,
                        CreatedDate = dispatched,
                        UpdatedDate = dispatched,
                        ArchivedDate = archived
                    });
                }
            }

            Console.WriteLine(
                $"Inserting {events.Count} events / {listenerEvents.Count} listener events and " +
                $"{eventArchives.Count} archived events / {listenerEventArchives.Count} archived " +
                $"listener events on {newReleases.Name} ({addressListeners.Count} listeners), " +
                $"backdated across the past {PastDays} days...");

            await broker.BulkInsertEventV2sAsync(events);
            await broker.BulkInsertListenerEventV2sAsync(listenerEvents);
            await broker.BulkInsertEventArchiveV2sAsync(eventArchives);
            await broker.BulkInsertListenerEventArchiveV2sAsync(listenerEventArchives);

            Console.WriteLine("Hydration complete.");
        }

        // Returns the existing NFlix participant (matched by the well-known Id), or adds it with
        // the fixed Id when it is missing.
        private static async Task<EventParticipantV2> EnsureParticipantAsync(
            EventHighwayClient client,
            DateTimeOffset now)
        {
            return await client.V2.EventParticipantV2Client
                .RetrieveOrAddEventParticipantV2Async(
                    new EventParticipantV2
                    {
                        Id = NFlixParticipantId,
                        Name = NFlixParticipantName,
                        Description = "NFlix streaming platform.",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });
        }

        // Returns the existing NFlix-NewReleases address (matched by the well-known Id), or
        // registers it with the fixed Id when it is missing.
        private static async Task<EventAddressV2> EnsureAddressAsync(
            EventHighwayClient client,
            DateTimeOffset now)
        {
            return await client.V2.EventAddressV2Client
                .RetrieveOrRegisterEventAddressV2Async(
                    new EventAddressV2
                    {
                        Id = NewReleasesAddressId,
                        Name = NewReleasesAddressName,
                        Description = "NFlix New Releases",
                        CreatedDate = now,
                        UpdatedDate = now
                    });
        }

        // Registers the BasicApp listeners (and their owning participants) on the address when they
        // are missing, then returns every listener on the address. Re-uses the well-known Guids so
        // it is idempotent and reconciles with the sample console apps.
        private static async Task<List<EventListenerV2>> EnsureListenersAsync(
            EventHighwayClient client,
            Guid eventAddressId,
            DateTimeOffset now)
        {
            foreach (ListenerSpec spec in ListenerSpecs)
            {
                await client.V2.EventParticipantV2Client
                    .RetrieveOrAddEventParticipantV2Async(
                        new EventParticipantV2
                        {
                            Id = spec.ParticipantId,
                            Name = spec.ParticipantName,
                            Description = spec.ParticipantDescription,
                            IsActive = true,
                            CreatedDate = now,
                            UpdatedDate = now
                        });

                await client.V2.EventListenerV2Client
                    .RetrieveOrRegisterEventListenerV2Async(
                        new EventListenerV2
                        {
                            Id = spec.ListenerId,
                            Name = spec.ListenerName,
                            Description = spec.ListenerDescription,
                            HandlerId = spec.HandlerId,
                            HandlerName = spec.HandlerName,
                            EventAddressV2Id = eventAddressId,
                            EventParticipantV2Id = spec.ParticipantId,
                            PromotedProperties = spec.PromotedProperties,
                            FilterCriteria = spec.FilterCriteria,
                            CreatedDate = now,
                            UpdatedDate = now
                        });
            }

            return (await client.V2.EventListenerV2Client
                .RetrieveEventListenerV2sByEventAddressIdAsync(eventAddressId))
                .ToList();
        }

        private static string BuildContent(Random rng) =>
            "{\"Title\":\"AddNewRelease\",\"Type\":\"Movie\",\"Rating\":\""
                + (rng.Next(10, 100) / 10.0).ToString("0.0") + "\"}";

        // Rolls the per-event outcome: ~80% all listeners succeed, ~8% partial success,
        // ~6% complete failure, remaining ~6% quarantined by loop detection.
        private static EventOutcome RollOutcome(Random rng)
        {
            int roll = rng.Next(0, 100);

            return roll < AllSuccessPercent ? EventOutcome.AllSuccess
                : roll < AllSuccessPercent + PartialSuccessPercent ? EventOutcome.PartialSuccess
                : roll < AllSuccessPercent + PartialSuccessPercent + AllFailurePercent
                    ? EventOutcome.AllFailure
                    : EventOutcome.Quarantined;
        }

        // Picks which listeners error for the outcome: none, 1..(n-1) for partial, or all.
        private static HashSet<int> PickErrorIndexes(
            Random rng,
            int listenerCount,
            EventOutcome outcome)
        {
            int errorCount = outcome switch
            {
                EventOutcome.AllSuccess => 0,
                EventOutcome.AllFailure => listenerCount,
                _ => rng.Next(1, Math.Max(2, listenerCount))
            };

            var errorIndexes = new HashSet<int>();

            while (errorIndexes.Count < errorCount)
            {
                errorIndexes.Add(rng.Next(0, listenerCount));
            }

            return errorIndexes;
        }

        // Errored deliveries carry a remaining-retry distribution so the Retry Health tiles have
        // dead (0), critical (1-2) and healthy (3+) populations: 50% dead, 25% critical, 25% healthy.
        private static int RandomErrorRemaining(Random rng)
        {
            int roll = rng.Next(0, 100);

            return roll < 50 ? 0
                : roll < 75 ? rng.Next(1, 3)
                : rng.Next(3, RetryAttemptsAllowed + 1);
        }

        private static DateTimeOffset RandomPastMoment(Random rng, DateTimeOffset now) =>
            now
                .AddDays(-rng.Next(0, PastDays))
                .AddHours(-rng.Next(0, 24))
                .AddMinutes(-rng.Next(0, 60))
                .AddSeconds(-rng.Next(0, 60));

        private enum EventOutcome
        {
            AllSuccess,
            PartialSuccess,
            AllFailure,
            Quarantined
        }

        private sealed record ListenerSpec(
            Guid ParticipantId,
            string ParticipantName,
            string ParticipantDescription,
            Guid ListenerId,
            string ListenerName,
            string ListenerDescription,
            Guid HandlerId,
            string HandlerName,
            string? PromotedProperties,
            string? FilterCriteria);
    }
}
