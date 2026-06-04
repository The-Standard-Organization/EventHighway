// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        [Fact]
        public async Task ShouldFireScheduledPendingEventV1sAsync()
        {
            // given
            string inputMockEndpoint = this.wireMockServer.Url;

            EventAddressV1 randomEventAddressV1 =
                await CreateRandomEventAddressV1Async();

            EventAddressV1 inputEventAddressV1 =
                randomEventAddressV1;

            Guid inputEventAddressV1Id =
                inputEventAddressV1.Id;

            IQueryable<EventV1> randomEventV1s =
                await CreateRandomEventV1sAsync(
                    inputEventAddressV1Id);

            IQueryable<EventV1> storageEventV1s =
                randomEventV1s;

            IQueryable<EventListenerV1> randomEventListenerV1s =
                await CreateRandomEventListenerV1sAsync(
                    inputEventAddressV1Id,
                    inputMockEndpoint);

            IQueryable<EventListenerV1> storageEventListenerV1s =
                randomEventListenerV1s;

            this.wireMockServer.Given(
                Request.Create()
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(code: HttpStatusCode.OK));

            // when
            await this.clientBroker.FireScheduledPendingEventV1sAsync();

            IQueryable<ListenerEventV1> storageListenerEventV1s =
                await this.clientBroker.RetrieveAllListenerEventV1s();

            // then
            foreach (ListenerEventV1 storageListenerEventV1
                in storageListenerEventV1s)
            {
                storageListenerEventV1s
                    .Where(listenerEventV1 =>
                        listenerEventV1.EventAddressId == inputEventAddressV1Id)
                            .Should().OnlyContain(listenerEventV1 =>
                                listenerEventV1.Status == ListenerEventStatusV1.Success);

                await this.clientBroker.RemoveListenerEventV1ByIdAsync(
                    storageListenerEventV1.Id);
            }

            foreach (EventV1 eventV1 in storageEventV1s)
            {
                await this.clientBroker.RemoveEventV1ByIdAsync(
                    eventV1.Id);
            }

            foreach (EventListenerV1 eventListenerV1 in storageEventListenerV1s)
            {
                await this.clientBroker.RemoveEventListenerV1ByIdAsync(
                    eventListenerV1.Id);
            }

            await this.clientBroker.RemoveEventAddressV1ByIdAsync(
                inputEventAddressV1Id);
        }
    }
}
