// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1Id = Guid.Empty;

            var invalidEventV1Exception =
                new InvalidEventV1CoordinationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Id),
                values: "Required");

            var expectedEventV1CoordinationValidationException =
                new EventV1CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1CoordinationService
                    .RemoveEventByIdAsync(
                        invalidEventV1Id);

            EventV1CoordinationValidationException
                actualEventV1CoordinationValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationValidationException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1CoordinationValidationException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationValidationException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.Verify(broker =>
                broker.RemoveEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
