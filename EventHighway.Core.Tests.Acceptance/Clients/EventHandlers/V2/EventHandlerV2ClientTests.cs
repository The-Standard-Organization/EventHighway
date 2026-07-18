// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Tests.Acceptance.Brokers;
using EventHighway.EventHandlers;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventHandlers.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventHandlerV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventHandlerV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private static DelegateEventHandler CreateRandomDelegateEventHandler() =>
            new DelegateEventHandler(
                Guid.NewGuid(),
                (_, _) => ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = "OK",
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                }),
                name: $"EventHandlerV2ClientTestsHandler-{Guid.NewGuid()}");
    }
}
