// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerV1IsNullAndLogItAsync()
        {
            // given
            EventListenerV1 nullEventListenerV1 = null;

            var nullEventListenerV1OrchestrationException =
                new NullEventListenerV1OrchestrationException(message: "Event listener is null.");

            var expectedEventListenerV1OrchestrationValidationException =
                new EventListenerV1OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerV1OrchestrationException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1OrchestrationService.AddEventListenerAsync(nullEventListenerV1);

            EventListenerV1OrchestrationValidationException
                actualEventListenerV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationValidationException>(
                        addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationValidationException))),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventListenerV1Async(
                    It.IsAny<EventListenerV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
