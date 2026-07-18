// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventParticipantSecretV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2 nullEventParticipantSecretV2 = null;

            var nullEventParticipantSecretV2Exception =
                new NullEventParticipantSecretV2Exception(message: "Event participant secret is null.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantSecretV2Exception);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    nullEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventParticipantSecretV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventParticipantSecretV2 = new EventParticipantSecretV2
            {
                Id = Guid.Empty,
                Secret = invalidText
            };

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.Id),
                values: "Required");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.CreatedDate),
                values: "Required");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.UpdatedDate),

                values:
                [
                    "Required",
                    $"Date is the same as {nameof(EventParticipantSecretV2.CreatedDate)}.",
                    "Date is not recent"
                ]);

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.EventParticipantV2Id),
                values: "Required");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(GetRandomDateTimeOffset());

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()),
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
            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTimeOffset);
            EventParticipantSecretV2 invalidEventParticipantSecretV2 = randomEventParticipantSecretV2;

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.UpdatedDate),
                values: $"Date is the same as {nameof(EventParticipantSecretV2.CreatedDate)}.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetDateTimeOffsetAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int secondsBeforeOrAfter)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTimeOffset);
            EventParticipantSecretV2 invalidEventParticipantSecretV2 = randomEventParticipantSecretV2;
            invalidEventParticipantSecretV2.CreatedDate = randomDateTimeOffset.AddDays(randomDaysAgo);
            invalidEventParticipantSecretV2.UpdatedDate = randomDateTimeOffset.AddSeconds(secondsBeforeOrAfter);

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.UpdatedDate),
                values: "Date is not recent");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventParticipantSecretV2DoesNotExistAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            EventParticipantSecretV2 nonExistingEventParticipantSecretV2 = randomEventParticipantSecretV2;
            nonExistingEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            EventParticipantSecretV2 nullEventParticipantSecretV2 = null;

            var notFoundEventParticipantSecretV2Exception =
                new NotFoundEventParticipantSecretV2Exception(
                    message: $"Could not find event participant secret with id: {nonExistingEventParticipantSecretV2.Id}.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    nonExistingEventParticipantSecretV2.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullEventParticipantSecretV2);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    nonExistingEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    nonExistingEventParticipantSecretV2.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()),
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
            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            EventParticipantSecretV2 invalidEventParticipantSecretV2 = randomEventParticipantSecretV2;
            invalidEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            DateTimeOffset randomOtherDateTime = GetRandomDateTimeOffset();
            EventParticipantSecretV2 storageEventParticipantSecretV2 = invalidEventParticipantSecretV2.DeepClone();
            storageEventParticipantSecretV2.CreatedDate = randomOtherDateTime;

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.CreatedDate),
                values: "Date is not the same as storage.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    invalidEventParticipantSecretV2.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEventParticipantSecretV2);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    invalidEventParticipantSecretV2.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldOverrideSecretFromStorageWhenSuppliedSecretNotSameAsStorageOnModifyAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomDaysAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTime);

            EventParticipantSecretV2 inputEventParticipantSecretV2 = randomEventParticipantSecretV2;
            inputEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);

            EventParticipantSecretV2 storageEventParticipantSecretV2 =
                inputEventParticipantSecretV2.DeepClone();

            string storedSecret = storageEventParticipantSecretV2.Secret;

            // the caller supplies a Secret that differs from storage; it must be overridden with
            // the stored value (the secret is immutable), so modify succeeds instead of failing.
            inputEventParticipantSecretV2.Secret = GetRandomString();

            EventParticipantSecretV2 persistedEventParticipantSecretV2 =
                inputEventParticipantSecretV2.DeepClone();

            persistedEventParticipantSecretV2.Secret = storedSecret;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                persistedEventParticipantSecretV2.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2.Id, randomCancellationToken))
                        .ReturnsAsync(storageEventParticipantSecretV2);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.Is<EventParticipantSecretV2>(secret => secret.Secret == storedSecret),
                    randomCancellationToken))
                        .ReturnsAsync(persistedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Service
                    .ModifyEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2);

            actualEventParticipantSecretV2.Secret.Should().Be(storedSecret);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2.Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.Is<EventParticipantSecretV2>(secret => secret.Secret == storedSecret),
                    randomCancellationToken),
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
            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            EventParticipantSecretV2 invalidEventParticipantSecretV2 = randomEventParticipantSecretV2;
            invalidEventParticipantSecretV2.CreatedDate = earlierDateTime;
            EventParticipantSecretV2 storageEventParticipantSecretV2 = invalidEventParticipantSecretV2.DeepClone();
            DateTimeOffset earlierSeconds = randomDateTime.AddSeconds(randomTimeAgo);
            invalidEventParticipantSecretV2.UpdatedDate = earlierSeconds;

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.UpdatedDate),
                values: "Date is earlier than storage.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    invalidEventParticipantSecretV2.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEventParticipantSecretV2);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    invalidEventParticipantSecretV2.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
