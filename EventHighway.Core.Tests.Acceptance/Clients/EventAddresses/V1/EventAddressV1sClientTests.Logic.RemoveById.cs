// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V1
{
    public partial class EventAddressV1sClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventAddressV1ByIdAsync()
        {
            // given
            EventAddressV1 randomEventAddressV1 =
                await CreateRandomEventAddressV1Async();

            EventAddressV1 inputEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                inputEventAddressV1.DeepClone();

            Guid inputEventAddressV1Id =
                inputEventAddressV1.Id;

            // when 
            EventAddressV1 actualEventAddressV1 =
                await this.clientBroker
                    .RemoveEventAddressV1ByIdAsync(
                        inputEventAddressV1Id);

            // then
            actualEventAddressV1.Should()
                .BeEquivalentTo(expectedEventAddressV1);
        }
    }
}
