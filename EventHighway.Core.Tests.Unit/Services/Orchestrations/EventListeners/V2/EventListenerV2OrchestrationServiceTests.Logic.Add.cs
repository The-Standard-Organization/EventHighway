// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddEventListenerV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2();

            EventListenerV2 inputEventListenerV2 =
                randomEventListenerV2;

            EventListenerV2 addedEventListenerV2 =
                inputEventListenerV2;

            EventListenerV2 expectedEventListenerV2 =
                addedEventListenerV2.DeepClone();

            this.eventListenerV2ProcessingServiceMock.Setup(broker =>
                broker.AddEventListenerV2Async(
                    inputEventListenerV2,
                    randomCancellationToken))
                        .ReturnsAsync(addedEventListenerV2);

            // when
            EventListenerV2 actualEventListenerV2 =
                await this.eventListenerV2OrchestrationService
                    .AddEventListenerV2Async(
                        inputEventListenerV2,
                        randomCancellationToken);

            // then
            actualEventListenerV2.Should().BeEquivalentTo(
                expectedEventListenerV2);

            this.eventListenerV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventListenerV2Async(
                    inputEventListenerV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.listenerEventV2ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
