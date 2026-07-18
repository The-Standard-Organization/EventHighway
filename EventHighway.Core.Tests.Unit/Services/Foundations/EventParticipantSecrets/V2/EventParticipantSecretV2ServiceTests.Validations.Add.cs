// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantSecretV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2 nullEventParticipantSecretV2 = null;

            var nullEventParticipantSecretV2Exception =
                new NullEventParticipantSecretV2Exception(
                    message: "Event participant secret is null.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantSecretV2Exception);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    nullEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantSecretV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventParticipantSecretV2 = new EventParticipantSecretV2
            {
                Id = Guid.Empty,
                Secret = invalidText,
            };

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.Id),
                values: "Required");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.Secret),
                values: "Required");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.CreatedDate),
                values: "Required");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.UpdatedDate),
                values: "Required");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.EventParticipantV2Id),
                values: "Required");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTimeOffset =
                randomDateTimeOffset.AddMinutes(GetRandomNegativeNumber());

            EventParticipantSecretV2 invalidEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(invalidDateTimeOffset);


            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.CreatedDate),
                values: "Date is not recent");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfActiveToIsNotAfterActiveFromAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset activeFrom = randomDateTimeOffset;
            DateTimeOffset activeTo = randomDateTimeOffset.AddDays(-1);

            EventParticipantSecretV2 invalidEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTimeOffset);

            invalidEventParticipantSecretV2.ActiveFrom = activeFrom;
            invalidEventParticipantSecretV2.ActiveTo = activeTo;

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.ActiveTo),
                values: $"Date must be after {nameof(EventParticipantSecretV2.ActiveFrom)}");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfActiveToIsSetToDefaultAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventParticipantSecretV2 invalidEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTimeOffset);

            invalidEventParticipantSecretV2.ActiveTo = default(DateTimeOffset);

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.ActiveTo),
                values: "Required");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfActiveFromIsSetToDefaultAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventParticipantSecretV2 invalidEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTimeOffset);

            invalidEventParticipantSecretV2.ActiveFrom = default(DateTimeOffset);

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.ActiveFrom),
                values: "Required");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset anotherRandomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventParticipantSecretV2 invalidEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTimeOffset);

            invalidEventParticipantSecretV2.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.CreatedDate),
                values: $"Date is not the same as {nameof(EventParticipantSecretV2.UpdatedDate)}");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfSecretIsTooShortAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventParticipantSecretV2 invalidEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTimeOffset);

            invalidEventParticipantSecretV2.Secret = new string('a', 35);

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.Secret),
                values: "Text must be at least 36 characters long");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    invalidEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

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
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
