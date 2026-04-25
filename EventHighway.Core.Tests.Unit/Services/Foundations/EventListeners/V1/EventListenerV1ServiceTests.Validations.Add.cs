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
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerV1IsNullAndLogItAsync()
        {
            // given
            EventListenerV1 nullEventListenerV1 = null;

            var nullEventListenerV1Exception =
                new NullEventListenerV1Exception(message: "Event listener is null.");

            var expectedEventListenerV1ValidationException =
                new EventListenerV1ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerV1Exception);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerAsync(nullEventListenerV1);

            EventListenerV1ValidationException actualEventListenerV1ValidationException =
                await Assert.ThrowsAsync<EventListenerV1ValidationException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventListenerV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            var invalidEventListenerV1 = new EventListenerV1
            {
                Id = Guid.Empty,
                Name = invalidText,
                Description = invalidText,
                HeaderSecret = invalidText,
                Endpoint = invalidText,
                EventAddressId = Guid.Empty
            };

            var invalidEventListenerV1Exception =
                new InvalidEventListenerV1Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.Id),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.Name),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.Description),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.HeaderSecret),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.Endpoint),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.EventAddressId),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.CreatedDate),
                values: "Required");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.UpdatedDate),
                values: "Required");

            var expectedEventListenerV1ValidationException =
                new EventListenerV1ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1Exception);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerAsync(invalidEventListenerV1);

            EventListenerV1ValidationException actualEventListenerV1ValidationException =
                await Assert.ThrowsAsync<EventListenerV1ValidationException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
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
            EventListenerV1 randomEventListenerV1 = CreateRandomEventListenerV1(dates: randomDateTimeOffset);
            EventListenerV1 invalidEventListenerV1 = randomEventListenerV1;
            invalidEventListenerV1.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventListenerV1Exception =
                new InvalidEventListenerV1Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.CreatedDate),
                values: $"Date is not the same as {nameof(EventListenerV1.UpdatedDate)}");

            var expectedEventListenerV1ValidationException =
                new EventListenerV1ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerAsync(invalidEventListenerV1);

            EventListenerV1ValidationException actualEventListenerV1ValidationException =
                await Assert.ThrowsAsync<EventListenerV1ValidationException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1(randomDateTimeOffset
                    .AddMinutes(minutesBeforeAndAfter));

            EventListenerV1 invalidEventListenerV1 = randomEventListenerV1;

            var invalidEventListenerV1Exception =
                new InvalidEventListenerV1Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1Exception.AddData(
                key: nameof(EventListenerV1.CreatedDate),
                values: "Date is not recent");

            var expectedEventListenerV1ValidationException =
                new EventListenerV1ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerAsync(invalidEventListenerV1);

            EventListenerV1ValidationException actualEventListenerV1ValidationException =
                await Assert.ThrowsAsync<EventListenerV1ValidationException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
