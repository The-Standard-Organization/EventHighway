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

namespace EventHighway.Core.Tests.Acceptance.Clients.ListenerEvents.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class ListenerEventV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;
        private readonly DelegateEventHandler delegateEventHandler;

        public ListenerEventV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();

            this.delegateEventHandler = new DelegateEventHandler(
                Guid.NewGuid(),
                (_, _) => ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = "OK",
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                }),
                name: $"ListenerEventV2ClientTestsHandler-{Guid.NewGuid()}");

            this.clientBroker = clientBroker;
            this.clientBroker.RegisterEventHandler(this.delegateEventHandler);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1).GetValue();

        private async ValueTask<IReadOnlyList<ListenerEventV2>> CreateRandomListenerEventV2sAsync()
        {
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            EventParticipantV2 randomEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            Guid inputEventParticipantV2Id = randomEventParticipantV2.Id;

            await CreateRandomEventListenerV2sAsync(inputEventAddressV2Id, inputEventParticipantV2Id);
            await CreateRandomEventV2sAsync(inputEventAddressV2Id, inputEventParticipantV2Id);

            return await RetrieveAllListenerEventV2sUntilAsync(
                listenerEventV2 => listenerEventV2.EventAddressV2Id == inputEventAddressV2Id);
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

        private async ValueTask CreateRandomEventListenerV2sAsync(
            Guid eventAddressV2Id,
            Guid eventParticipantV2Id)
        {
            int randomNumber = GetRandomNumber();

            for (int index = 0; index < randomNumber; index++)
            {
                EventListenerV2 listenerV2 =
                    CreateDelegateHandlerListenerV2(eventAddressV2Id, eventParticipantV2Id);

                await this.clientBroker.RegisterEventListenerV2Async(listenerV2);
            }
        }

        private async ValueTask CreateRandomEventV2sAsync(
            Guid eventAddressV2Id,
            Guid eventParticipantV2Id)
        {
            int randomNumber = GetRandomNumber();

            for (int index = 0; index < randomNumber; index++)
            {
                EventV2 randomEventV2 = CreateEventV2Filler(
                    eventAddressV2Id,
                    eventParticipantV2Id)
                        .Create();

                await this.clientBroker.SubmitEventV2Async(randomEventV2);
            }
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private async ValueTask<EventParticipantV2> CreateRandomEventParticipantV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                CreateEventParticipantV2Filler().Create();

            await this.clientBroker.AddEventParticipantV2Async(
                randomEventParticipantV2);

            return randomEventParticipantV2;
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

        private static Filler<EventV2> CreateEventV2Filler(
            Guid eventAddressV2Id,
            Guid eventParticipantV2Id,
            DateTimeOffset? scheduledDate = null)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnProperty(eventV2 => eventV2.EventAddressV2).IgnoreIt()
                .OnProperty(eventV2 => eventV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventAddressV2Id).Use(eventAddressV2Id)
                .OnProperty(eventV2 => eventV2.ScheduledDate).Use(scheduledDate)
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(eventV2 => eventV2.EventParticipantV2Id).Use(eventParticipantV2Id)
                .OnProperty(eventV2 => eventV2.EventParticipantV2Secret).IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

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
    }
}
