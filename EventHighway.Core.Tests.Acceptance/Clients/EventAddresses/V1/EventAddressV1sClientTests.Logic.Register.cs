// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V1
{
    public partial class EventAddressV1sClientTests
    {
        [Fact]
        public async Task ShouldRegisterEventAddressV1Async()
        {
            // given
            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            EventAddressV1 inputEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                inputEventAddressV1.DeepClone();

            // when 
            EventAddressV1 actualEventAddressV1 =
                await this.clientBroker
                    .RegisterEventAddressV1Async(
                        inputEventAddressV1);

            // then
            actualEventAddressV1.Should()
                .BeEquivalentTo(expectedEventAddressV1);

            await this.clientBroker
                .RemoveEventAddressV1ByIdAsync(
                    inputEventAddressV1.Id);
        }
    }
}
