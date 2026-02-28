// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V1
{
    public partial class EventListenerV1sClientTests
    {
        [Fact]
        public async Task ShouldRegisterEventListenerV1Async()
        {
            // given
            EventAddressV1 randomEventAddressV1 =
                await CreateRandomEventAddressV1Async();

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1(
                    randomEventAddressV1.Id);

            EventListenerV1 inputEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                inputEventListenerV1.DeepClone();

            // when 
            EventListenerV1 actualEventListenerV1 =
                await this.clientBroker
                    .RegisterEventListenerV1Async(
                        inputEventListenerV1);

            // then
            actualEventListenerV1.Should()
                .BeEquivalentTo(expectedEventListenerV1);

            await this.clientBroker
                .RemoveEventListenerV1ByIdAsync(
                    inputEventListenerV1.Id);

            await this.clientBroker
                .RemoveEventAddressV1ByIdAsync(
                    randomEventAddressV1.Id);
        }
    }
}
