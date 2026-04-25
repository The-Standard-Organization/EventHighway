// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V1
{
    public partial class EventAddressV1ServiceTests
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
                randomEventAddressV1s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventAddressesV1Async())
                    .ReturnsAsync(retrievedEventAddressV1s);

            // when
            IQueryable<EventAddressV1> actualEventAddressV1s =
                await this.eventAddressV1Service
                    .RetrieveAllEventAddressesAsync();

            // then
            actualEventAddressV1s.Should().BeEquivalentTo(
                expectedEventAddressV1s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventAddressesV1Async(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
