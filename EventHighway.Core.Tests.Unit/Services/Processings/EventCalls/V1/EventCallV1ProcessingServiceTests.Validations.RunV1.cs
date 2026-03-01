// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Processings.EventCalls.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V1
{
    public partial class EventCallV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunV1IfEventCallV1IsNullAndLogItAsync()
        {
            // given
            EventCallV1 nullEventCallV1 = null;

            var nullEventCallV1ProcessingException =
                new NullEventCallV1ProcessingException(message: "Event call is null.");

            var expectedEventCallV1ProcessingValidationException =
                new EventCallV1ProcessingValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: nullEventCallV1ProcessingException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1ProcessingService.RunEventCallV1AsyncV1(nullEventCallV1);

            EventCallV1ProcessingValidationException
                actualEventCallV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventCallV1ProcessingValidationException>(
                        runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventCallV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ProcessingValidationException))),
                        Times.Once);

            this.eventCallV1ServiceMock.Verify(broker =>
                broker.RunEventCallV1AsyncV1(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
