// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventV1sWithListenersAsync()
        {
            // given
            IQueryable<EventV1> randomEventV1s =
                CreateRandomEventV1s();

            IQueryable<EventV1> retrievedEventV1s =
                randomEventV1s;

            IQueryable<EventV1> expectedEventV1s =
                retrievedEventV1s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventV1sWithListenersAsync())
                    .ReturnsAsync(retrievedEventV1s);

            // when
            IQueryable<EventV1> actualEventV1s =
                await this.eventV1Service
                    .RetrieveAllEventV1sWithListenersAsync();

            // then
            actualEventV1s.Should().BeEquivalentTo(
                expectedEventV1s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV1sWithListenersAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
