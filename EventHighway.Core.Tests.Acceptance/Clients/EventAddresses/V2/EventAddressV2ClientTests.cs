// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventAddressV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventAddressV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private async ValueTask<IQueryable<EventAddressV2>> CreateRandomEventAddressV2sAsync()
        {
            int randomCount = GetRandomNumber();
            var randomEventAddressV2s = new List<EventAddressV2>();

            for (int index = 1; index <= randomCount; index++)
            {
                EventAddressV2 randomEventAddressV2 =
                    CreateRandomEventAddressV2();

                await this.clientBroker.RegisterEventAddressV2Async(
                    randomEventAddressV2);

                randomEventAddressV2s.Add(randomEventAddressV2);
            }

            return randomEventAddressV2s.AsQueryable();
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateRandomEventAddressV2();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private static EventAddressV2 CreateRandomEventAddressV2() =>
            CreateEventAddressV2Filler().Create();

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            DateTimeOffset now = TruncateToMicroseconds(DateTimeOffset.UtcNow);
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

        private static DateTimeOffset TruncateToMicroseconds(
            DateTimeOffset dateTimeOffset)
        {
            long ticksToRemove = dateTimeOffset.Ticks % TimeSpan.TicksPerMicrosecond;

            return dateTimeOffset.AddTicks(-ticksToRemove);
        }
    }
}
