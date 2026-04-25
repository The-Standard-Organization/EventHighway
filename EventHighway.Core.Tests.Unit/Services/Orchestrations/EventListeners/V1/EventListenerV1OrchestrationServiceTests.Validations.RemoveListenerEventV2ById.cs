// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationOnRemoveListenerEventIdByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventAddressId = Guid.Empty;

            var invalidEventListenerV1OrchestrationException =
                new InvalidEventListenerV1OrchestrationException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1OrchestrationException.AddData(
                key: nameof(ListenerEventV1.Id),
                values: "Required");

            var expectedEventListenerV1OrchestrationValidationException =
                new EventListenerV1OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1OrchestrationException);

            // when
            ValueTask<ListenerEventV1> removeEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1OrchestrationService.RemoveListenerEventByIdAsync(
                    invalidEventAddressId);

            EventListenerV1OrchestrationValidationException
                actualEventListenerV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationValidationException>(
                        removeEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
