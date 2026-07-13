// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V2
{
    public partial class EventListenerV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveOrRegisterEventListenerV2Async()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2(
                    randomEventAddressV2.Id);

            EventListenerV2 inputEventListenerV2 =
                randomEventListenerV2;

            EventListenerV2 expectedEventListenerV2 =
                inputEventListenerV2.DeepClone();

            // when
            EventListenerV2 actualEventListenerV2 =
                await this.clientBroker
                    .RetrieveOrRegisterEventListenerV2Async(
                        inputEventListenerV2);

            // then
            actualEventListenerV2.Should()
                .BeEquivalentTo(expectedEventListenerV2);

            await this.clientBroker
                .RemoveEventListenerV2ByIdAsync(
                    inputEventListenerV2.Id);

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(
                    randomEventAddressV2.Id);
        }
    }
}
