// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V1
{
    public partial class EventListenerV1sClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventListenerV1ByIdAsync()
        {
            // given
            EventListenerV1 randomEventListenerV1 =
                await CreateRandomEventListenerV1Async();

            EventListenerV1 inputEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                inputEventListenerV1.DeepClone();

            Guid inputEventListenerV1Id =
                inputEventListenerV1.Id;

            // when 
            EventListenerV1 actualEventListenerV1 =
                await this.clientBroker
                    .RemoveEventListenerV1ByIdAsync(
                        inputEventListenerV1Id);

            // then
            actualEventListenerV1.Should()
                .BeEquivalentTo(expectedEventListenerV1);

            await this.clientBroker
                .RemoveEventAddressV1ByIdAsync(
                    inputEventListenerV1.EventAddressId);
        }
    }
}
