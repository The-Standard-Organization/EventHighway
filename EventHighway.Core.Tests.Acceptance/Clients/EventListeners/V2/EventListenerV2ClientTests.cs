// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventListenerV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventListenerV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateRandomEventAddressV2();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private async ValueTask<EventListenerV2> CreateRandomEventListenerV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            EventParticipantV2 randomEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2(
                    randomEventAddressV2.Id,
                    randomEventParticipantV2.Id);

            await this.clientBroker.RegisterEventListenerV2Async(
                randomEventListenerV2);

            return randomEventListenerV2;
        }

        private async ValueTask<EventParticipantV2> CreateRandomEventParticipantV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                CreateEventParticipantV2Filler().Create();

            await this.clientBroker.AddEventParticipantV2Async(
                randomEventParticipantV2);

            return randomEventParticipantV2;
        }

        private static EventListenerV2 CreateRandomEventListenerV2(
            Guid eventAddressId,
            Guid eventParticipantId) =>
                CreateEventListenerV2Filler(eventAddressId, eventParticipantId).Create();

        private static EventAddressV2 CreateRandomEventAddressV2() =>
            CreateEventAddressV2Filler().Create();

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnProperty(eventAddressV2 =>
                    eventAddressV2.EventV2s).IgnoreIt()

                .OnProperty(eventAddressV2 =>
                    eventAddressV2.EventListenerV2s).IgnoreIt()

                .OnProperty(eventAddressV2 =>
                    eventAddressV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventAddressV2 =>
                    eventAddressV2.EventArchiveV2s).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }

        private static Filler<EventListenerV2> CreateEventListenerV2Filler(
            Guid eventAddressId,
            Guid eventParticipantId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filler = new Filler<EventListenerV2>();

            filler.Setup()
                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventAddressV2Id).Use(eventAddressId)

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.ListenerEventArchiveV2s).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventParticipantV2Id).Use(eventParticipantId)

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventParticipantV2).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }

        private static Filler<EventParticipantV2> CreateEventParticipantV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.Id).Use(() => Guid.NewGuid())

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.IsActive).Use(true)

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ActiveFrom).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ActiveTo).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventArchiveV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventListenerV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ListenerEventArchiveV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventParticipantSecretV2s).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }
    }
}
