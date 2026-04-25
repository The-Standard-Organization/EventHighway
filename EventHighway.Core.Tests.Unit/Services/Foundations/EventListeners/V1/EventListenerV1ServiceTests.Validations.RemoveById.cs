// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventListenerV1Id = Guid.Empty;

            var invalidEventListenerV1Exception =
                new InvalidEventListenerV1Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.Id),
                values: "Required");

            var expectedEventListenerV1ValidationException =
                new EventListenerV1ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1Exception);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1Service.RemoveEventListenerV1ByIdAsync(
                    invalidEventListenerV1Id);

            EventListenerV1ValidationException actualEventListenerV1ValidationException =
                await Assert.ThrowsAsync<EventListenerV1ValidationException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerByIdV1Async(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventListenerV1IsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingEventListenerV1Id = GetRandomId();
            EventListenerV1 nullEventListenerV1 = null;

            var notFoundEventListenerV1Exception =
                new NotFoundEventListenerV1Exception(
                    message: $"Could not find event listener with id: {nonExistingEventListenerV1Id}.");

            var expectedEventListenerV1ValidationException =
                new EventListenerV1ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventListenerV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventListenerByIdV1Async(It.IsAny<Guid>()))
                    .ReturnsAsync(nullEventListenerV1);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1Service.RemoveEventListenerV1ByIdAsync(
                    nonExistingEventListenerV1Id);

            EventListenerV1ValidationException actualEventListenerV1ValidationException =
                await Assert.ThrowsAsync<EventListenerV1ValidationException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerByIdV1Async(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventListenerV1Async(
                    It.IsAny<EventListenerV1>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
