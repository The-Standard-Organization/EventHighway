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
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1Id = Guid.Empty;

            var invalidEventV1Exception =
                new InvalidEventV1OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Id),
                values: "Required");

            var expectedEventV1OrchestrationValidationException =
                new EventV1OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1OrchestrationService
                    .RemoveEventByIdAsync(
                        invalidEventV1Id);

            EventV1OrchestrationValidationException
                actualEventV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationValidationException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationValidationException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.RemoveEventByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
