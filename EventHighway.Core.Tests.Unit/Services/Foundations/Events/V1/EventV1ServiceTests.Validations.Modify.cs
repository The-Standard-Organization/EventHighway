// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventV1IsNullAndLogItAsync()
        {
            // given
            EventV1 nullEventV1 = null;

            var nullEventV1Exception =
                new NullEventV1Exception(message: "Event is null.");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1Exception);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(nullEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should().BeEquivalentTo(
                expectedEventV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnModifyIfEventV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            EventTypeV1 invalidEventV1Type = GetInvalidEnum<EventTypeV1>();

            var invalidEventV1 = new EventV1
            {
                Id = Guid.Empty,
                Content = invalidText,
                Type = invalidEventV1Type,
                EventAddressId = Guid.Empty
            };

            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Id),
                values: "Required");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Content),
                values: "Required");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.EventAddressId),
                values: "Required");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.Type),
                values: "Value is not recognized");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.CreatedDate),
                values: "Required");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.UpdatedDate),

                values:
                [
                    "Required",
                    $"Date is the same as {nameof(EventV1.CreatedDate)}."
                ]);

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(invalidEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should().BeEquivalentTo(
                expectedEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV1 randomEventV1 = CreateRandomEventV1(randomDateTimeOffset);
            EventV1 invalidEventV1 = randomEventV1;

            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.UpdatedDate),
                values: $"Date is the same as {nameof(EventV1.CreatedDate)}.");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(invalidEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetDateTimeOffsetAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV1 randomEventV1 = CreateRandomEventV1(randomDateTimeOffset);
            EventV1 invalidEventV1 = randomEventV1;

            invalidEventV1.UpdatedDate =
                invalidEventV1.UpdatedDate.AddMinutes(minutesBeforeOrAfter);

            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.UpdatedDate),
                values: "Date is not recent");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(invalidEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventV1DoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventV1 randomEventV1 = CreateRandomEventV1(randomDateTime);
            EventV1 nonExistingEventV1 = randomEventV1;
            nonExistingEventV1.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            EventV1 nullEventV1 = null;

            var notFoundEventV1Exception =
                new NotFoundEventV1Exception(
                    message: $"Could not find event with id: {nonExistingEventV1.Id}.");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ByIdAsync(nonExistingEventV1.Id))
                    .ReturnsAsync(nullEventV1);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(nonExistingEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(nonExistingEventV1.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationErrorOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomDaysAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            EventV1 randomEventV1 = CreateRandomEventV1(randomDateTime);
            EventV1 invalidEventV1 = randomEventV1;
            invalidEventV1.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            DateTimeOffset randomOtherDateTime = GetRandomDateTimeOffset();
            EventV1 storageEventV1 = invalidEventV1.DeepClone();
            storageEventV1.CreatedDate = randomOtherDateTime;

            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.CreatedDate),
                values: $"Date is not the same as storage.");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ByIdAsync(invalidEventV1.Id))
                    .ReturnsAsync(storageEventV1);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(invalidEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(
                    modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(invalidEventV1.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsEarlierThanStorageAndLogItAsync()
        {
            // given
            int randomTimeAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset earlierDateTime = randomDateTime.AddDays(randomTimeAgo);
            EventV1 randomEventV1 = CreateRandomEventV1(randomDateTime);
            EventV1 invalidEventV1 = randomEventV1;
            invalidEventV1.CreatedDate = earlierDateTime;
            EventV1 storageEventV1 = invalidEventV1.DeepClone();
            DateTimeOffset earlierSeconds = randomDateTime.AddSeconds(randomTimeAgo);
            invalidEventV1.UpdatedDate = earlierSeconds;

            var invalidEventV1Exception =
                new InvalidEventV1Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(EventV1.UpdatedDate),
                values: $"Date is earlier than storage.");

            var expectedEventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ByIdAsync(invalidEventV1.Id))
                    .ReturnsAsync(storageEventV1);

            // when
            ValueTask<EventV1> modifyEventV1Task =
                this.eventV1Service.ModifyEventV1Async(invalidEventV1);

            EventV1ValidationException actualEventV1ValidationException =
                await Assert.ThrowsAsync<EventV1ValidationException>(modifyEventV1Task.AsTask);

            // then
            actualEventV1ValidationException.Should()
                .BeEquivalentTo(expectedEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ByIdAsync(invalidEventV1.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
