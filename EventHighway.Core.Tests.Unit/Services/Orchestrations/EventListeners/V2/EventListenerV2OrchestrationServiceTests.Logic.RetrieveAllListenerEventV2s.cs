// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventV2> randomListenerEventV2s =
                CreateRandomListenerEventV2s();

            IQueryable<ListenerEventV2> retrievedListenerEventV2s =
                randomListenerEventV2s;

            IQueryable<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s.DeepClone();

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync())
                    .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IQueryable<ListenerEventV2> actualListenerEventV2s =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(
                expectedListenerEventV2s);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventListenerV2ProcessingServiceMock
                .VerifyNoOtherCalls();
                        this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
