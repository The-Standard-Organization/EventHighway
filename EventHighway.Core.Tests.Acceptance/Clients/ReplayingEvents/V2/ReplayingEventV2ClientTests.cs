// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Tests.Acceptance.Brokers;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.ReplayingEvents.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class ReplayingEventV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public ReplayingEventV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }
    }
}
