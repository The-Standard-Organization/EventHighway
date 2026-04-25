// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V1
{
    public partial class EventListenerV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventListenerV1Id = Guid.Empty;

            var invalidEventListenerV1Exception =
                new InvalidEventListenerV1ProcessingException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.Id),
                values: "Required");

            var expectedEventListenerV1ProcessingValidationException =
                new EventListenerV1ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1Exception);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1ProcessingService
                    .RemoveEventListenerV1ByIdAsync(
                        invalidEventListenerV1Id);

            EventListenerV1ProcessingValidationException
                actualEventListenerV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventListenerV1ProcessingValidationException>(
                        removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingValidationException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.Verify(broker =>
                broker.RemoveEventListenerByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
