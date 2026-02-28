// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V1
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventAddressV1sClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventAddressV1sClientTests()
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = new ClientBroker();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private async ValueTask<IQueryable<EventAddressV1>> CreateRandomEventAddressV1sAsync()
        {
            int randomCount = GetRandomNumber();
            var randomEventAddressV1s = new List<EventAddressV1>();

            for (int index = 1; index <= randomCount; index++)
            {
                EventAddressV1 randomEventAddressV1 =
                    CreateRandomEventAddressV1();

                await this.clientBroker.RegisterEventAddressV1Async(
                    randomEventAddressV1);

                randomEventAddressV1s.Add(randomEventAddressV1);
            }

            return randomEventAddressV1s.AsQueryable();
        }

        private async ValueTask<EventAddressV1> CreateRandomEventAddressV1Async()
        {
            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            await this.clientBroker.RegisterEventAddressV1Async(
                randomEventAddressV1);

            return randomEventAddressV1;
        }

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
    }
}
