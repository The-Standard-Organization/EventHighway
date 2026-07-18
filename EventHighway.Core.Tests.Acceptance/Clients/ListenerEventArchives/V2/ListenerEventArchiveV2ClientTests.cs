// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.ListenerEventArchives.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class ListenerEventArchiveV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public ListenerEventArchiveV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
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
