// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

// Reusable database hydrator for the EventHighway sample data. Inserts backdated EventV2 +
// ListenerEventV2 rows directly through the internal StorageBroker, bypassing foundation
// validation (which forbids a backdated CreatedDate). Any console app that references this
// project can call DatabaseHydrator.HydrateNewReleasesAsync(connectionString) to top up the
// database. Self-sufficient: the Ensure* helpers create the NFlix participant, the
// NFlix-NewReleases address and its listeners when missing, so it runs against an empty database
// on its own; when the ClientV2.SubstrateApp/BasicApp samples have already run it reconciles to
// their rows via the shared well-known Guids instead of duplicating them.

using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

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

        // BingeBox / Joe / Ann participants + listeners — the same set EventHighway.ClientV2.BasicApp
        // registers (same Guids as the shared SeedIdentifiers), so runs reconcile to the same rows.
        // Handler Ids are fixed here (BasicApp mints random ones at runtime) so the seeded listener
        // rows are stable across re-runs; the handlers themselves are in-memory dispatch objects.
        private static readonly ListenerSpec[] ListenerSpecs =
        {
            new ListenerSpec(
                ParticipantId: new Guid("72edb46a-4e55-49dc-8b92-16baf040c6fd"),
                ParticipantName: "BingeBox",
                ParticipantDescription: "BingeBox a NFlix affiliate",
                ListenerId: new Guid("07864612-508c-4177-a0b6-061f9efa48d8"),
                ListenerName: "BingeBox New Releases Listener",
                ListenerDescription: "Receives every NFlix new release.",
                HandlerId: new Guid("6326cae3-04ff-411f-93fb-e606859390f6"),
                HandlerName: "BingeBox",
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

        // Volume targets (in addition to whatever already exists).
        private const int ActiveEventCount = 500;
        private const int QuarantinedEventCount = 50;
        private const int DeadEventCount = 50;

        private const int PastDays = 30;

        public static async Task HydrateNewReleasesAsync(string connectionString)
        {
            var broker = new StorageBroker(connectionString);
            DateTimeOffset setupNow = DateTimeOffset.UtcNow;

            EventParticipantV2 nflix =
                await EnsureParticipantAsync(broker, setupNow);

            EventAddressV2 newReleases =
                await EnsureAddressAsync(broker, setupNow);

            List<EventListenerV2> addressListeners =
                await EnsureListenersAsync(broker, newReleases.Id, setupNow);

            // Fixed seed so re-runs are reproducible.
            var rng = new Random(20260701);
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var events = new List<EventV2>();
            var listenerEvents = new List<ListenerEventV2>();

            // ---- 500 active events with one ListenerEventV2 per listener ----
            // Split into thirds: all-success, all-failure, mixed.
            int allSuccessUpperBound = ActiveEventCount / 3;
            int allFailureUpperBound = (ActiveEventCount / 3) * 2;

            for (int index = 0; index < ActiveEventCount; index++)
            {
                DateTimeOffset created = RandomPastMoment(rng, now);

                bool isScheduled = rng.Next(0, 2) == 0;

                var eventV2 = BuildEvent(
                    rng,
                    newReleases.Id,
                    nflix.Id,
                    created,
                    status: EventStatusV2.Active,
                    type: isScheduled ? EventTypeV2.Scheduled : EventTypeV2.Immediate,
                    scheduledDate: isScheduled ? created : (DateTimeOffset?)null,
                    remainingRetryAttempts: rng.Next(2, 6));

                events.Add(eventV2);

                Outcome outcome =
                    index < allSuccessUpperBound ? Outcome.AllSuccess
                    : index < allFailureUpperBound ? Outcome.AllFailure
                    : Outcome.Mixed;

                foreach (EventListenerV2 listener in addressListeners)
                {
                    bool isError = outcome switch
                    {
                        Outcome.AllSuccess => false,
                        Outcome.AllFailure => true,
                        _ => rng.Next(0, 2) == 0
                    };

                    listenerEvents.Add(BuildListenerEvent(eventV2, listener, isError));
                }
            }

            // ---- 50 quarantined events ----
            for (int index = 0; index < QuarantinedEventCount; index++)
            {
                DateTimeOffset created = RandomPastMoment(rng, now);
                bool isScheduled = rng.Next(0, 2) == 0;

                events.Add(BuildEvent(
                    rng,
                    newReleases.Id,
                    nflix.Id,
                    created,
                    status: EventStatusV2.Quarantined,
                    type: isScheduled ? EventTypeV2.Scheduled : EventTypeV2.Immediate,
                    scheduledDate: isScheduled ? created : (DateTimeOffset?)null,
                    remainingRetryAttempts: rng.Next(2, 6)));
            }

            // ---- 50 immediate, 0-retry (dead) events ----
            for (int index = 0; index < DeadEventCount; index++)
            {
                DateTimeOffset created = RandomPastMoment(rng, now);

                events.Add(BuildEvent(
                    rng,
                    newReleases.Id,
                    nflix.Id,
                    created,
                    status: EventStatusV2.Active,
                    type: EventTypeV2.Immediate,
                    scheduledDate: null,
                    remainingRetryAttempts: 0));
            }

            Console.WriteLine(
                $"Inserting {events.Count} events and {listenerEvents.Count} listener events " +
                $"on {newReleases.Name} ({addressListeners.Count} listeners)...");

            await broker.BulkInsertEventV2sAsync(events);
            await broker.BulkInsertListenerEventV2sAsync(listenerEvents);

            Console.WriteLine("Hydration complete.");
        }

        // Returns the existing NFlix participant (matched by the well-known Id or name), or inserts
        // it with the fixed Id when it is missing.
        private static async Task<EventParticipantV2> EnsureParticipantAsync(
            StorageBroker broker,
            DateTimeOffset now)
        {
            List<EventParticipantV2> participants =
                (await broker.SelectAllEventParticipantV2sAsync()).ToList();

            EventParticipantV2? existing =
                participants.FirstOrDefault(participant => participant.Id == NFlixParticipantId)
                    ?? participants.FirstOrDefault(participant =>
                        participant.Name == NFlixParticipantName);

            if (existing is not null)
            {
                return existing;
            }

            Console.WriteLine($"Creating missing participant '{NFlixParticipantName}'...");

            return await broker.InsertEventParticipantV2Async(
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

        // Returns the existing NFlix-NewReleases address (matched by the well-known Id or name), or
        // inserts it with the fixed Id when it is missing.
        private static async Task<EventAddressV2> EnsureAddressAsync(
            StorageBroker broker,
            DateTimeOffset now)
        {
            List<EventAddressV2> addresses =
                (await broker.SelectAllEventAddressV2sAsync()).ToList();

            EventAddressV2? existing =
                addresses.FirstOrDefault(address => address.Id == NewReleasesAddressId)
                    ?? addresses.FirstOrDefault(address => address.Name == NewReleasesAddressName);

            if (existing is not null)
            {
                return existing;
            }

            Console.WriteLine($"Creating missing address '{NewReleasesAddressName}'...");

            return await broker.InsertEventAddressV2Async(
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
            StorageBroker broker,
            Guid eventAddressId,
            DateTimeOffset now)
        {
            List<EventParticipantV2> participants =
                (await broker.SelectAllEventParticipantV2sAsync()).ToList();

            List<EventListenerV2> listeners =
                (await broker.SelectAllEventListenerV2sAsync()).ToList();

            foreach (ListenerSpec spec in ListenerSpecs)
            {
                if (participants.All(participant => participant.Id != spec.ParticipantId))
                {
                    Console.WriteLine($"Creating missing participant '{spec.ParticipantName}'...");

                    await broker.InsertEventParticipantV2Async(
                        new EventParticipantV2
                        {
                            Id = spec.ParticipantId,
                            Name = spec.ParticipantName,
                            Description = spec.ParticipantDescription,
                            IsActive = true,
                            CreatedDate = now,
                            UpdatedDate = now
                        });
                }

                if (listeners.All(listener => listener.Id != spec.ListenerId))
                {
                    Console.WriteLine($"Creating missing listener '{spec.ListenerName}'...");

                    await broker.InsertEventListenerV2Async(
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
            }

            return (await broker.SelectAllEventListenerV2sAsync())
                .Where(listener => listener.EventAddressV2Id == eventAddressId)
                .ToList();
        }

        private static EventV2 BuildEvent(
            Random rng,
            Guid eventAddressId,
            Guid participantId,
            DateTimeOffset created,
            EventStatusV2 status,
            EventTypeV2 type,
            DateTimeOffset? scheduledDate,
            int remainingRetryAttempts) =>
            new EventV2
            {
                Id = Guid.NewGuid(),
                Content = "{\"Title\":\"AddNewRelease\",\"Type\":\"Movie\",\"Rating\":\""
                    + (rng.Next(10, 100) / 10.0).ToString("0.0") + "\"}",
                EventName = NewReleaseEventName,
                ContentHash = "HASH-" + Guid.NewGuid().ToString("N")[..12],
                Type = type,
                Status = status,
                ScheduledDate = scheduledDate,
                EventAddressV2Id = eventAddressId,
                EventParticipantV2Id = participantId,
                EventParticipantV2Secret = "NFlix",
                CreatedDate = created,
                UpdatedDate = created
            };

        private static ListenerEventV2 BuildListenerEvent(
            EventV2 eventV2,
            EventListenerV2 listener,
            bool isError) =>
            new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = isError ? ListenerEventStatusV2.Error : ListenerEventStatusV2.Success,
                Response = isError ? "Handler failed" : "Event received",
                ResponseCode = isError ? "503" : "200",
                ResponseMessage = isError ? "Service Unavailable" : "OK",
                EventV2Id = eventV2.Id,
                EventAddressV2Id = eventV2.EventAddressV2Id,
                EventListenerV2Id = listener.Id,
                EventParticipantV2Id = listener.EventParticipantV2Id,
                CreatedDate = eventV2.CreatedDate.AddSeconds(3),
                UpdatedDate = eventV2.CreatedDate.AddSeconds(3)
            };

        private static DateTimeOffset RandomPastMoment(Random rng, DateTimeOffset now) =>
            now
                .AddDays(-rng.Next(0, PastDays))
                .AddHours(-rng.Next(0, 24))
                .AddMinutes(-rng.Next(0, 60))
                .AddSeconds(-rng.Next(0, 60));

        private enum Outcome
        {
            AllSuccess,
            AllFailure,
            Mixed
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
