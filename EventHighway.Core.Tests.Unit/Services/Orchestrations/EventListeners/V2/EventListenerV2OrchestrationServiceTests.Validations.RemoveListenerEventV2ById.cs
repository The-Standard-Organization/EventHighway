// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnRemoveListenerEventV2ByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidListenerEventV2Id = Guid.Empty;

            var invalidEventListenerV2OrchestrationException =
                new InvalidEventListenerV2OrchestrationException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2OrchestrationException.AddData(
                key: nameof(ListenerEventV2.Id),
                values: "Required");

            var expectedEventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2OrchestrationException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.eventListenerV2OrchestrationService
                    .RemoveListenerEventV2ByIdAsync(
                        invalidListenerEventV2Id,
                        randomCancellationToken);

            EventListenerV2OrchestrationValidationException
                actualEventListenerV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationValidationException>(
                        removeListenerEventV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
                        this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
