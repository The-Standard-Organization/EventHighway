// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveScheduledPendingEventV1sAsync()
        {
            // given
            IQueryable<EventV1> randomEventV1s = CreateRandomEventV1s();
            IQueryable<EventV1> retrievedEventV1s = randomEventV1s;

            IQueryable<EventV1> expectedEventV1s = retrievedEventV1s
                .DeepClone();

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventsAsync())
                    .ReturnsAsync(retrievedEventV1s);

            // when
            IQueryable<EventV1> actualEventV1s =
                await this.eventV1OrchestrationService
                    .RetrieveScheduledPendingEventsAsync();

            // then
            actualEventV1s.Should().BeEquivalentTo(expectedEventV1s);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventsAsync(),
                    Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
