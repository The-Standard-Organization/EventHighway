// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyListenerEventV2IfNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 nullListenerEventV2 = null;

            var nullListenerEventV2OrchestrationException =
                new NullListenerEventV2OrchestrationException(message: "Listener event is null.");

            var expectedEventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV2OrchestrationException);

            // when
            ValueTask<ListenerEventV2> modifyListenerEventV2Task =
                this.eventListenerV2OrchestrationService.ModifyListenerEventV2Async(
                    nullListenerEventV2,
                    randomCancellationToken);

            EventListenerV2OrchestrationValidationException
                actualEventListenerV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationValidationException>(
                        modifyListenerEventV2Task.AsTask);

            // then
            actualEventListenerV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(broker =>
                broker.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
                        this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
