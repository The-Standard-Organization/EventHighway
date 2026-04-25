// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V1
{
    public partial class EventV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnMarkAsImmediateIfEventV1IsNullAndLogItAsync()
        {
            // given
            EventV1 nullEventV1 = null;

            var nullEventV1ProcessingException =
                new NullEventV1ProcessingException(message: "Event is null.");

            var expectedEventV1ProcessingValidationException =
                new EventV1ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ProcessingException);

            // when
            ValueTask<EventV1> markEventV1AsImmediateTask =
                this.eventV1ProcessingService.MarkEventV1AsImmediateAsync(nullEventV1);

            EventV1ProcessingValidationException
                actualEventV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventV1ProcessingValidationException>(
                        markEventV1AsImmediateTask.AsTask);

            // then
            actualEventV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventV1ServiceMock.Verify(broker =>
                broker.ModifyEventAsync(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
