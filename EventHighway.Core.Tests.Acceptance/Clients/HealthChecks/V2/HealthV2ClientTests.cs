// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using EventHighway.EventHandlers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class HealthV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;
        private readonly DelegateEventHandler delegateEventHandler;

        public HealthV2ClientTests()
        {
            this.wireMockServer = WireMockServer.Start();

            this.delegateEventHandler = new DelegateEventHandler(
                Guid.NewGuid(),
                (content, cancellationToken) =>
                {
                    string[] parts = content.Split(',');
                    int sum = int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());

                    return ValueTask.FromResult(new EventHandlerResult
                    {
                        IsSuccess = true,
                        Response = sum.ToString(),
                        ResponseCode = "200",
                        ResponseMessage = "OK"
                    });
                },
                name: $"HealthV2ClientTestsHandler-{Guid.NewGuid()}");

            this.clientBroker = new ClientBroker();

            this.clientBroker
                .RegisterEventHandler(this.delegateEventHandler);
        }

        private async ValueTask<IReadOnlyList<ListenerEventV2>>
            RetrieveAllListenerEventV2sUntilAsync(
                Func<ListenerEventV2, bool> predicate)
        {
            IReadOnlyList<ListenerEventV2> listenerEventV2s =
                await this.clientBroker.RetrieveAllListenerEventV2sAsync(
                    new ListenerEventV2Query { Take = 1000 });

            for (int retries = 0; retries < 20 && !listenerEventV2s.Any(predicate); retries++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250));

                listenerEventV2s =
                    await this.clientBroker.RetrieveAllListenerEventV2sAsync(
                    new ListenerEventV2Query { Take = 1000 });
            }

            return listenerEventV2s;
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private EventListenerV2 CreateDelegateHandlerListenerV2(
            Guid eventAddressId,
            Guid eventParticipantId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            return new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerId = this.delegateEventHandler.Id,
                HandlerName = this.delegateEventHandler.Name,
                EventAddressV2Id = eventAddressId,
                EventParticipantV2Id = eventParticipantId,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }

        private async ValueTask<EventParticipantV2> CreateRandomEventParticipantV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                CreateEventParticipantV2Filler().Create();

            await this.clientBroker.AddEventParticipantV2Async(
                randomEventParticipantV2);

            return randomEventParticipantV2;
        }

        private async ValueTask<EventV2> SubmitScheduledEventV2Async(
            Guid eventAddressV2Id,
            Guid eventParticipantV2Id,
            string content = null)
        {
            EventV2 eventV2 = CreateEventV2Filler(
                eventAddressV2Id,
                eventParticipantV2Id,
                scheduledDate: DateTimeOffset.UtcNow.AddSeconds(1),
                content: content)
                    .Create();

            await this.clientBroker.SubmitEventV2Async(eventV2);
            await Task.Delay(TimeSpan.FromSeconds(2));

            return eventV2;
        }

        private static DateTimeOffset GetCurrentDayWindowStart() =>
            new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);

        private static DateTimeOffset GetCurrentHourStart()
        {
            DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

            return new DateTimeOffset(
                nowUtc.Year, nowUtc.Month, nowUtc.Day, nowUtc.Hour, 0, 0, TimeSpan.Zero);
        }

        private async ValueTask<SeededEventV2> SeedFiredEventV2Async()
        {
            int randomNumberA = GetRandomPositiveInt();
            int randomNumberB = GetRandomPositiveInt();
            string content = $"{randomNumberA},{randomNumberB}";
            string expectedResponse = $"{randomNumberA + randomNumberB}";

            EventAddressV2 eventAddressV2 =
                await CreateRandomEventAddressV2Async();

            EventParticipantV2 eventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventListenerV2 listenerV2 =
                CreateDelegateHandlerListenerV2(eventAddressV2.Id, eventParticipantV2.Id);

            await this.clientBroker.RegisterEventListenerV2Async(listenerV2);

            EventV2 eventV2 =
                await SubmitScheduledEventV2Async(
                    eventAddressV2.Id, eventParticipantV2.Id, content: content);

            await this.clientBroker.FireScheduledPendingEventV2sAsync();

            IReadOnlyList<ListenerEventV2> listenerEventV2s =
                await RetrieveAllListenerEventV2sUntilAsync(listenerEventV2 =>
                    listenerEventV2.EventV2Id == eventV2.Id &&
                    listenerEventV2.Status == ListenerEventStatusV2.Success &&
                    listenerEventV2.Response == expectedResponse);

            return new SeededEventV2
            {
                EventAddressV2 = eventAddressV2,
                EventListenerV2 = listenerV2,
                EventV2 = eventV2,
                ListenerEventV2s = listenerEventV2s
                    .Where(listenerEventV2 => listenerEventV2.EventV2Id == eventV2.Id)
                    .ToList(),
            };
        }

        private async ValueTask CleanupSeededEventV2Async(SeededEventV2 seededEventV2)
        {
            foreach (ListenerEventV2 listenerEventV2 in seededEventV2.ListenerEventV2s)
                await this.clientBroker.RemoveListenerEventV2ByIdAsync(listenerEventV2.Id);

            await this.clientBroker.RemoveEventV2ByIdAsync(seededEventV2.EventV2.Id);
            await this.clientBroker.RemoveEventListenerV2ByIdAsync(seededEventV2.EventListenerV2.Id);
            await this.clientBroker.RemoveEventAddressV2ByIdAsync(seededEventV2.EventAddressV2.Id);
        }

        private sealed class SeededEventV2
        {
            public EventAddressV2 EventAddressV2 { get; init; }
            public EventListenerV2 EventListenerV2 { get; init; }
            public EventV2 EventV2 { get; init; }
            public List<ListenerEventV2> ListenerEventV2s { get; init; }
        }

        private static int GetRandomPositiveInt() =>
            new IntRange(min: 1, max: 100).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1).GetValue();

        private static Filler<EventParticipantV2> CreateEventParticipantV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnProperty(eventParticipantV2 => eventParticipantV2.Id).Use(() => Guid.NewGuid())
                .OnProperty(eventParticipantV2 => eventParticipantV2.IsActive).Use(true)
                .OnProperty(eventParticipantV2 => eventParticipantV2.IsSecretRequired).Use(false)
                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveFrom).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveTo).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventArchiveV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventListenerV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventArchiveV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventParticipantSecretV2s).IgnoreIt()
                .OnType<DateTimeOffset>().Use(now);

            return filler;
        }

        private static Filler<EventV2> CreateEventV2Filler(
            Guid eventAddressV2Id,
            Guid eventParticipantV2Id,
            DateTimeOffset scheduledDate,
            string content = null)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnProperty(eventV2 => eventV2.EventAddressV2).IgnoreIt()
                .OnProperty(eventV2 => eventV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventAddressV2Id).Use(eventAddressV2Id)
                .OnProperty(eventV2 => eventV2.ScheduledDate).Use(scheduledDate)
                .OnProperty(eventV2 => eventV2.Status).Use(EventStatusV2.Active)
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(eventV2 => eventV2.EventParticipantV2Id).Use(eventParticipantV2Id)
                .OnProperty(eventV2 => eventV2.EventParticipantV2Secret).IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt();

            if (content is not null)
                filler.Setup().OnProperty(eventV2 => eventV2.Content).Use(content);

            return filler;
        }

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(eventAddressV2 => eventAddressV2.EventV2s).IgnoreIt()
                .OnProperty(eventAddressV2 => eventAddressV2.EventListenerV2s).IgnoreIt()
                .OnProperty(eventAddressV2 => eventAddressV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventAddressV2 => eventAddressV2.EventArchiveV2s).IgnoreIt();

            return filler;
        }
    }
}
