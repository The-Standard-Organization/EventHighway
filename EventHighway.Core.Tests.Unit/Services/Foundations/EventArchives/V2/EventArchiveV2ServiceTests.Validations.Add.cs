// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2 nullEventArchiveV2 = null;

            var nullEventArchiveV2Exception =
                new NullEventArchiveV2Exception(
                    message: "Event archive is null.");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2Exception);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2Service.AddEventArchiveV2Async(
                    nullEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveTypeV2 invalidType = GetInvalidEnum<EventArchiveTypeV2>();

            var invalidEventArchiveV2 = new EventArchiveV2
            {
                Id = Guid.Empty,
                Type = invalidType,
                Content = invalidText,
                EventName = invalidText
            };

            var invalidEventArchiveV2Exception =
                new InvalidEventArchiveV2Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.Id),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.Content),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.ContentHash),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.EventName),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.Type),
                values: "Value is not recognized");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.CreatedDate),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.UpdatedDate),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.ArchivedDate),
                values: "Required");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.EventAddressV2Id),
                values: "Required");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2Exception);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2Service.AddEventArchiveV2Async(
                    invalidEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfArchiveDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfterNow)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventArchiveV2 randomEventArchiveV2 =
                CreateRandomEventArchiveV2(
                    date: randomDateTimeOffset.AddSeconds(minutesBeforeAndAfterNow));

            EventArchiveV2 invalidEventArchiveV2 = randomEventArchiveV2;

            var invalidEventArchiveV2Exception =
                new InvalidEventArchiveV2Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.ArchivedDate),
                values: "Date is not recent");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2Service.AddEventArchiveV2Async(
                    invalidEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            EventArchiveV2 randomEventArchiveV2 = CreateRandomEventArchiveV2(randomDateTimeOffset);
            EventArchiveV2 invalidEventArchiveV2 = randomEventArchiveV2;
            invalidEventArchiveV2.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventArchiveV2Exception =
                new InvalidEventArchiveV2Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.CreatedDate),
                values: $"Date is not the same as {nameof(EventArchiveV2.UpdatedDate)}");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2Service.AddEventArchiveV2Async(
                    invalidEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
