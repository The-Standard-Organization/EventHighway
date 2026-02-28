// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventAddresses.V1
{
    public partial class EventAddressV1sClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV1sAsync()
        {
            // given
            IQueryable<EventAddressV1> randomEventAddressV1s =
                await CreateRandomEventAddressV1sAsync();

            IQueryable<EventAddressV1> inputEventAddressV1s =
                randomEventAddressV1s;

            IQueryable<EventAddressV1> expectedEventAddressV1s =
                inputEventAddressV1s.DeepClone();

            // when 
            IQueryable<EventAddressV1> actualEventAddressV1s =
                await this.clientBroker
                    .RetrieveAllEventAddressV1sAsync();

            // then
            actualEventAddressV1s.Should()
                .BeEquivalentTo(expectedEventAddressV1s);

            foreach (EventAddressV1 actualEventAddressV1
                in actualEventAddressV1s)
            {
                await this.clientBroker
                    .RemoveEventAddressV1ByIdAsync(
                        actualEventAddressV1.Id);
            }
        }
    }
}
