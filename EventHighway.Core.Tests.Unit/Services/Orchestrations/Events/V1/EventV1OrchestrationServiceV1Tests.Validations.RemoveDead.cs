// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceV1Tests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveDeadEventsIfEventV1IsNullAndLogItAsync()
        {
            // given
            EventV1 nullEventV1 = null;

            var nullEventV1OrchestrationException =
                new NullEventV1OrchestrationException(
                    message: "Event is null.");

            var expectedEventV1OrchestrationValidationException =
                new EventV1OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1OrchestrationException);

            // when
            ValueTask removeEventV1AndListenerEventV1sTask =
                this.eventV1OrchestrationServiceV1.RemoveEventV1AndListenerEventV1sAsync(
                    nullEventV1);

            EventV1OrchestrationValidationException actualEventV1OrchestrationValidationException =
                await Assert.ThrowsAsync<EventV1OrchestrationValidationException>(
                    removeEventV1AndListenerEventV1sTask.AsTask);

            // then
            actualEventV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
