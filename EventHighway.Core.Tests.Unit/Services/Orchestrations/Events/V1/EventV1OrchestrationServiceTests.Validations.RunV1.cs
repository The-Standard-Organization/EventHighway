// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunV1IfEventCallV1IsNullAndLogItAsync()
        {
            // given
            EventCallV1 nullEventCallV1 = null;

            var nullEventCallV1OrchestrationException =
                new NullEventCallV1OrchestrationException(message: "Event call is null.");

            var expectedEventV1OrchestrationValidationException =
                new EventV1OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventCallV1OrchestrationException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventV1OrchestrationService.RunEventCallV1AsyncV1(nullEventCallV1);

            EventV1OrchestrationValidationException
                actualEventV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationValidationException>(
                        runEventCallV1Task.AsTask);

            // then
            actualEventV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationValidationException))),
                        Times.Once);

            this.eventCallV1ProcessingServiceMock.Verify(broker =>
                broker.RunEventCallV1AsyncV1(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
