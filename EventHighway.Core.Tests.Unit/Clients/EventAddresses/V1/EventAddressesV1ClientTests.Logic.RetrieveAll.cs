// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V1
{
    public partial class EventAddressesV1ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV1sAsync()
        {
            // given
            IQueryable<EventAddressV1> randomEventAddressV1s =
                CreateRandomEventAddressV1s();

            IQueryable<EventAddressV1> retrievedEventAddressV1s =
                randomEventAddressV1s;

            IQueryable<EventAddressV1> expectedEventAddressV1s =
                retrievedEventAddressV1s.DeepClone();

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressesAsync())
                    .ReturnsAsync(retrievedEventAddressV1s);

            // when
            IQueryable<EventAddressV1> actualEventAddressV1s =
                await this.eventAddressV1sClient
                    .RetrieveAllEventAddressV1sAsync();

            // then
            actualEventAddressV1s.Should().BeEquivalentTo(
                expectedEventAddressV1s);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressesAsync(),
                    Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
