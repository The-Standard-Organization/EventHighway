// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V1
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventListenerV1sClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventListenerV1sClientTests()
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = new ClientBroker();
        }

        private async ValueTask<EventAddressV1> CreateRandomEventAddressV1Async()
        {
            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            await this.clientBroker.RegisterEventAddressV1Async(
                randomEventAddressV1);

            return randomEventAddressV1;
        }

        private async ValueTask<EventListenerV1> CreateRandomEventListenerV1Async()
        {
            EventAddressV1 randomEventAddressV1 =
                await CreateRandomEventAddressV1Async();

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1(
                    randomEventAddressV1.Id);

            await this.clientBroker.RegisterEventListenerV1Async(
                randomEventListenerV1);

            return randomEventListenerV1;
        }

        private static EventListenerV1 CreateRandomEventListenerV1(Guid eventAddressId) =>
            CreateEventListenerV1Filler(eventAddressId).Create();

        private static EventAddressV1 CreateRandomEventAddressV1() =>
            CreateEventAddressV1Filler().Create();

        private static Filler<EventAddressV1> CreateEventAddressV1Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventAddressV1>();

            filler.Setup()
                .OnProperty(eventV1 =>
                    eventV1.Events).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.ListenerEvents).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.EventListeners).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }

        private static Filler<EventListenerV1> CreateEventListenerV1Filler(
            Guid eventAddressId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventListenerV1>();

            filler.Setup()
                .OnProperty(eventV1 =>
                    eventV1.EventAddressId).Use(eventAddressId)

                .OnProperty(eventV1 =>
                    eventV1.EventAddress).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.ListenerEvents).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }
    }
}
