// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByEventAddressIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventAddressId = Guid.Empty;

            var invalidEventListenerV2OrchestrationException =
                new InvalidEventListenerV2OrchestrationException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2OrchestrationException.AddData(
                key: nameof(EventListenerV2.EventAddressId),
                values: "Required");

            var expectedEventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2OrchestrationException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sByEventAddressIdTask =
                this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        invalidEventAddressId,
                        randomCancellationToken);

            EventListenerV2OrchestrationValidationException
                actualEventListenerV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationValidationException>(
                        retrieveEventListenerV2sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.Verify(broker =>
                broker.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
                        this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
