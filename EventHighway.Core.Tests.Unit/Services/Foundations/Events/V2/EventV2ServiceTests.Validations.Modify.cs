// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 nullEventV2 = null;

            var nullEventV2Exception =
                new NullEventV2Exception(message: "Event is null.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV2Exception);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(nullEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnModifyIfEventV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventTypeV2 invalidEventV2Type = GetInvalidEnum<EventTypeV2>();

            var invalidEventV2 = new EventV2
            {
                Id = Guid.Empty,
                Content = invalidText,
                EventName = invalidText,
                Type = invalidEventV2Type,
                EventAddressV2Id = Guid.Empty
            };

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Id),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Content),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.ContentHash),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.EventName),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.EventAddressV2Id),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.EventParticipantV2Id),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Type),
                values: "Value is not recognized");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.CreatedDate),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.UpdatedDate),

                values:
                [
                    "Required",
                    $"Date is the same as {nameof(EventV2.CreatedDate)}."
                ]);

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(invalidEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2(randomDateTimeOffset);
            EventV2 invalidEventV2 = randomEventV2;

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.UpdatedDate),
                values: $"Date is the same as {nameof(EventV2.CreatedDate)}.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(invalidEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should()
                .BeEquivalentTo(expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetDateTimeOffsetAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventV2HasInvalidLengthPropertyAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventV2 invalidEventV2 = CreateRandomEventV2(dates: randomDateTimeOffset);
            invalidEventV2.CreatedDate = randomDateTimeOffset.AddDays(randomDaysAgo);
            invalidEventV2.EventName = GetRandomStringWithLengthOf(451);

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.EventName),
                values: $"Text exceed max length of {invalidEventV2.EventName.Length - 1} characters");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(invalidEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2(randomDateTimeOffset);
            EventV2 invalidEventV2 = randomEventV2;

            invalidEventV2.UpdatedDate =
                invalidEventV2.UpdatedDate.AddSeconds(minutesBeforeOrAfter);

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.UpdatedDate),
                values: "Date is not recent");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(invalidEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should()
                .BeEquivalentTo(expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventV2DoesNotExistAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventV2 randomEventV2 = CreateRandomEventV2(randomDateTime);
            EventV2 nonExistingEventV2 = randomEventV2;
            nonExistingEventV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            EventV2 nullEventV2 = null;

            var notFoundEventV2Exception =
                new NotFoundEventV2Exception(
                    message: $"Could not find event with id: {nonExistingEventV2.Id}.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(nonExistingEventV2.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(nullEventV2);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(nonExistingEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should()
                .BeEquivalentTo(expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(nonExistingEventV2.Id, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationErrorOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomDaysAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2(randomDateTime);
            EventV2 invalidEventV2 = randomEventV2;
            invalidEventV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            DateTimeOffset randomOtherDateTime = GetRandomDateTimeOffset();
            EventV2 storageEventV2 = invalidEventV2.DeepClone();
            storageEventV2.CreatedDate = randomOtherDateTime;

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.CreatedDate),
                values: $"Date is not the same as storage.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(invalidEventV2.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEventV2);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(invalidEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should()
                .BeEquivalentTo(expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(invalidEventV2.Id, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsEarlierThanStorageAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTimeAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset earlierDateTime = randomDateTime.AddDays(randomTimeAgo);
            EventV2 randomEventV2 = CreateRandomEventV2(randomDateTime);
            EventV2 invalidEventV2 = randomEventV2;
            invalidEventV2.CreatedDate = earlierDateTime;
            EventV2 storageEventV2 = invalidEventV2.DeepClone();
            DateTimeOffset earlierSeconds = randomDateTime.AddSeconds(randomTimeAgo);
            invalidEventV2.UpdatedDate = earlierSeconds;

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.UpdatedDate),
                values: $"Date is earlier than storage.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(invalidEventV2.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEventV2);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(invalidEventV2, randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(modifyEventV2Task.AsTask);

            // then
            actualEventV2ValidationException.Should()
                .BeEquivalentTo(expectedEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(invalidEventV2.Id, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
