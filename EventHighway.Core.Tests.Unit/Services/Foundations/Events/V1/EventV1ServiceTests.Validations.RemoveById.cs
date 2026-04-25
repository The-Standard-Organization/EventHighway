// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1Id = Guid.Empty;

            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Id),
                values: "Required");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1Service.RemoveEventByIdAsync(
                    invalidEventV1Id);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventByIdV1Async(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventV1IsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingEventV1Id = GetRandomId();
            EventV1 nullEventV1 = null;

            var notFoundEventV1Exception =
                new NotFoundEventV1Exception(
                    message: $"Could not find event with id: {nonExistingEventV1Id}.");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventByIdV1Async(It.IsAny<Guid>()))
                    .ReturnsAsync(nullEventV1);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1Service.RemoveEventByIdAsync(nonExistingEventV1Id);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventByIdV1Async(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1Async(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
