// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationOnRetrieveByEventAddressIdIfEventAddressIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventAddressId = Guid.Empty;

            var invalidEventListenerV1OrchestrationException =
                new InvalidEventListenerV1OrchestrationException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1OrchestrationException.AddData(
                key: nameof(EventListenerV1.EventAddressId),
                values: "Required");

            var expectedEventListenerV1OrchestrationValidationException =
                new EventListenerV1OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1OrchestrationException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1OrchestrationService.RetrieveEventListenersByEventAddressIdAsync(
                    invalidEventAddressId);

            EventListenerV1OrchestrationValidationException actualEventListenerV1OrchestrationValidationException =
                await Assert.ThrowsAsync<EventListenerV1OrchestrationValidationException>(
                    retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationValidationException))),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenersByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
