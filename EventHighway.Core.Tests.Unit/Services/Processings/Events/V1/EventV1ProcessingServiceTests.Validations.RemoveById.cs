// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1Id = Guid.Empty;

            var invalidEventV1Exception =
                new InvalidEventV1ProcessingException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Id),
                values: "Required");

            var expectedEventV1ProcessingValidationException =
                new EventV1ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1ProcessingService
                    .RemoveEventV1ByIdAsync(
                        invalidEventV1Id);

            EventV1ProcessingValidationException
                actualEventV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventV1ProcessingValidationException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingValidationException))),
                        Times.Once);

            this.eventV1ServiceMock.Verify(broker =>
                broker.RemoveEventByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
