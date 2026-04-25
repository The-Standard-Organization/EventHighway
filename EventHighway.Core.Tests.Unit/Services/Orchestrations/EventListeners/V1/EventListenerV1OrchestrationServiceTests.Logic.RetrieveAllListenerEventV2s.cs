// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventV1sAsync()
        {
            // given
            IQueryable<ListenerEventV1> randomListenerEventV1s =
                CreateRandomListenerEventV1s();

            IQueryable<ListenerEventV1> retrievedListenerEventV1s =
                randomListenerEventV1s;

            IQueryable<ListenerEventV1> expectedListenerEventV1s =
                retrievedListenerEventV1s.DeepClone();

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventsAsync())
                    .ReturnsAsync(retrievedListenerEventV1s);

            // when
            IQueryable<ListenerEventV1> actualListenerEventV1s =
                await this.eventListenerV1OrchestrationService
                    .RetrieveAllListenerEventsAsync();

            // then
            actualListenerEventV1s.Should().BeEquivalentTo(
                expectedListenerEventV1s);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventsAsync(),
                    Times.Once);

            this.listenerEventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventListenerV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
