// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V1
{
    public partial class EventAddressV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventAddressV1Id = Guid.Empty;

            var invalidEventAddressV1Exception =
                new InvalidEventAddressV1Exception(
                    message: "Event address is invalid, fix the errors and try again.");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.Id),
                values: "Required");

            var expectedEventAddressV1ValidationException =
                new EventAddressV1ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV1Exception);

            // when
            ValueTask<EventAddressV1> removeEventAddressV1ByIdTask =
                this.eventAddressV1Service.RemoveEventAddressByIdAsync(
                    invalidEventAddressV1Id);

            EventAddressV1ValidationException actualEventAddressV1ValidationException =
                await Assert.ThrowsAsync<EventAddressV1ValidationException>(
                    removeEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventAddressV1IsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingEventAddressV1Id = GetRandomId();
            EventAddressV1 nullEventAddressV1 = null;

            var notFoundEventAddressV1Exception =
                new NotFoundEventAddressV1Exception(
                    message: $"Could not find event address with id: {nonExistingEventAddressV1Id}.");

            var expectedEventAddressV1ValidationException =
                new EventAddressV1ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventAddressV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(nullEventAddressV1);

            // when
            ValueTask<EventAddressV1> removeEventAddressV1ByIdTask =
                this.eventAddressV1Service.RemoveEventAddressByIdAsync(nonExistingEventAddressV1Id);

            EventAddressV1ValidationException actualEventAddressV1ValidationException =
                await Assert.ThrowsAsync<EventAddressV1ValidationException>(
                    removeEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventAddressV1Async(
                    It.IsAny<EventAddressV1>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
