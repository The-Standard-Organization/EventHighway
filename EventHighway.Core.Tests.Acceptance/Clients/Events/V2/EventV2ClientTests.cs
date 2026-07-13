// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using EventHighway.EventHandlers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;
        private readonly DelegateEventHandler delegateEventHandler;

        public EventV2ClientTests(
            ClientBroker clientBroker)
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
                name: $"EventV2ClientTestsHandler-{Guid.NewGuid()}");

            this.clientBroker = clientBroker;

            this.clientBroker
                .RegisterEventHandler(this.delegateEventHandler);
        }

        private async ValueTask<IQueryable<ListenerEventV2>>
            RetrieveAllListenerEventV2sUntilAsync(
                Func<ListenerEventV2, bool> predicate)
        {
            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.clientBroker.RetrieveAllListenerEventV2sAsync();

            for (int retries = 0; retries < 20 && !listenerEventV2s.Any(predicate); retries++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250));

                listenerEventV2s =
                    await this.clientBroker.RetrieveAllListenerEventV2sAsync();
            }

            return listenerEventV2s;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static int GetRandomPositiveInt() =>
            new IntRange(min: 1, max: 100).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1).GetValue();

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private async ValueTask<EventV2> SubmitScheduledEventV2Async(
            Guid eventAddressV2Id,
            string content = null)
        {
            EventV2 eventV2 = CreateEventV2Filler(
                eventAddressV2Id,
                scheduledDate: DateTimeOffset.UtcNow.AddSeconds(1),
                content: content)
                    .Create();

            await this.clientBroker.SubmitEventV2Async(eventV2);
            await Task.Delay(TimeSpan.FromSeconds(2));

            return eventV2;
        }

        private async ValueTask<EventV2> SubmitEventV2Async(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate)
        {
            EventV2 eventV2 = CreateEventV2Filler(
                eventAddressV2Id,
                scheduledDate)
                    .Create();

            await this.clientBroker.SubmitEventV2Async(eventV2);

            return eventV2;
        }

        private EventV2 CreateRandomEventV2(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate)
        {
            return CreateEventV2Filler(
                eventAddressV2Id,
                scheduledDate)
                    .Create();
        }

        private EventListenerV2 CreateDelegateHandlerListenerV2(Guid eventAddressId)
        {
            DateTimeOffset now = 
                DateTimeOffset.UtcNow;

            return new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerId = this.delegateEventHandler.Id,
                HandlerName = this.delegateEventHandler.Name,
                EventAddressV2Id = eventAddressId,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }

        private Filler<EventV2> CreateEventV2Filler(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate,
            string content = null)
        {
            DateTimeOffset now = 
                DateTimeOffset.UtcNow;

            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnProperty(eventV2 => eventV2.EventAddressV2).IgnoreIt()
                .OnProperty(eventV2 => eventV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventAddressV2Id).Use(eventAddressV2Id)
                .OnProperty(eventV2 => eventV2.ScheduledDate).Use(scheduledDate)
                .OnProperty(eventV2 => eventV2.Status).Use(EventStatusV2.Active)
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(eventV2 => eventV2.EventParticipantV2Id).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventParticipantV2Secret).IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt();

            if (content is not null)
                filler.Setup().OnProperty(eventV2 => eventV2.Content).Use(content);

            return filler;
        }

        private Filler<EventAddressV2> CreateEventAddressV2Filler()
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

        private async ValueTask<EventParticipantV2> CreateRandomEventParticipantV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                CreateEventParticipantV2Filler().Create();

            return await this.clientBroker.AddEventParticipantV2Async(
                randomEventParticipantV2);
        }

        private async ValueTask<EventParticipantSecretV2> CreateRandomEventParticipantSecretV2Async(
            Guid participantId)
        {
            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateEventParticipantSecretV2Filler(participantId).Create();

            return await this.clientBroker.AddEventParticipantSecretV2Async(
                randomEventParticipantSecretV2);
        }

        private Filler<EventParticipantV2> CreateEventParticipantV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnProperty(eventParticipantV2 => eventParticipantV2.Id).Use(() => Guid.NewGuid())
                .OnProperty(eventParticipantV2 => eventParticipantV2.IsActive).Use(true)
                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveFrom).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveTo).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventArchiveV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventListenerV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventArchiveV2s).IgnoreIt()
                .OnProperty(eventParticipantV2 => eventParticipantV2.EventParticipantSecretV2s).IgnoreIt()
                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }

        private Filler<EventParticipantSecretV2> CreateEventParticipantSecretV2Filler(
            Guid participantId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventParticipantSecretV2>();

            filler.Setup()
                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.Id).Use(() => Guid.NewGuid())
                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.EventParticipantV2Id).Use(participantId)
                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.IsActive).Use(true)
                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.ActiveFrom).IgnoreIt()
                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.ActiveTo).IgnoreIt()
                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.EventParticipantV2).IgnoreIt()
                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }
    }
}
