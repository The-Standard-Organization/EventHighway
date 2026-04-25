// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnMarkAsImmediateIfEventV1IsNullAndLogItAsync()
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
            ValueTask<EventV1> markEventV1AsImmediateTask =
                this.eventV1OrchestrationService.MarkEventAsImmediateAsync(nullEventV1);

            EventV1OrchestrationValidationException
                actualEventV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationValidationException>(
                        markEventV1AsImmediateTask.AsTask);

            // then
            actualEventV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationValidationException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.MarkEventAsImmediateAsync(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
