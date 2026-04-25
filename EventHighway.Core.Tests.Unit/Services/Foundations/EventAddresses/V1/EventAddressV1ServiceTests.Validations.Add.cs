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
        public async Task ShouldThrowValidationExceptionOnAddIfEventAddressV1IsNullAndLogItAsync()
        {
            // given
            EventAddressV1 nullEventAddressV1 = null;

            var nullEventAddressV1Exception =
                new NullEventAddressV1Exception(message: "Event address is null.");

            var expectedEventAddressV1ValidationException =
                new EventAddressV1ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: nullEventAddressV1Exception);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(nullEventAddressV1);

            EventAddressV1ValidationException actualEventAddressV1ValidationException =
                await Assert.ThrowsAsync<EventAddressV1ValidationException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ValidationException.Should().BeEquivalentTo(
                expectedEventAddressV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventAddressV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            var invalidEventAddressV1 = new EventAddressV1
            {
                Id = Guid.Empty,
                Name = invalidText,
                Description = invalidText,
            };

            var invalidEventAddressV1Exception =
                new InvalidEventAddressV1Exception(
                    message: "Event address is invalid, fix the errors and try again.");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.Id),
                values: "Required");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.Name),
                values: "Required");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.Description),
                values: "Required");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.CreatedDate),
                values: "Required");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.UpdatedDate),
                values: "Required");

            var expectedEventAddressV1ValidationException =
                new EventAddressV1ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV1Exception);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(invalidEventAddressV1);

            EventAddressV1ValidationException actualEventAddressV1ValidationException =
                await Assert.ThrowsAsync<EventAddressV1ValidationException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ValidationException.Should().BeEquivalentTo(
                expectedEventAddressV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            EventAddressV1 randomEventAddressV1 = CreateRandomEventAddressV1(dates: randomDateTimeOffset);
            EventAddressV1 invalidEventAddressV1 = randomEventAddressV1;
            invalidEventAddressV1.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventAddressV1Exception =
                new InvalidEventAddressV1Exception(
                    message: "Event address is invalid, fix the errors and try again.");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.CreatedDate),
                values: $"Date is not the same as {nameof(EventAddressV1.UpdatedDate)}");

            var expectedEventAddressV1ValidationException =
                new EventAddressV1ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(invalidEventAddressV1);

            EventAddressV1ValidationException actualEventAddressV1ValidationException =
                await Assert.ThrowsAsync<EventAddressV1ValidationException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ValidationException.Should().BeEquivalentTo(
                expectedEventAddressV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfterNow)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1(
                    dates: randomDateTimeOffset.AddMinutes(minutesBeforeAndAfterNow));

            EventAddressV1 invalidEventAddressV1 = randomEventAddressV1;

            var invalidEventAddressV1Exception =
                new InvalidEventAddressV1Exception(
                    message: "Event address is invalid, fix the errors and try again.");

            invalidEventAddressV1Exception.AddData(
                key: nameof(EventAddressV1.CreatedDate),
                values: "Date is not recent");

            var expectedEventAddressV1ValidationException =
                new EventAddressV1ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(invalidEventAddressV1);

            EventAddressV1ValidationException actualEventAddressV1ValidationException =
                await Assert.ThrowsAsync<EventAddressV1ValidationException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ValidationException.Should().BeEquivalentTo(
                expectedEventAddressV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
