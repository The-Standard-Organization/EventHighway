// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV1IsNullAndLogItAsync()
        {
            // given
            EventArchiveV1 nullEventArchiveV1 = null;

            var nullEventArchiveV1Exception =
                new NullEventArchiveV1Exception(
                    message: "Event archive is null.");

            var expectedEventArchiveV1ValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV1Exception);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(nullEventArchiveV1);

            EventArchiveV1ValidationException actualEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            EventArchiveTypeV1 invalidType = GetInvalidEnum<EventArchiveTypeV1>();

            var invalidEventArchiveV1 = new EventArchiveV1
            {
                Id = Guid.Empty,
                Type = invalidType,
                Content = invalidText
            };

            var invalidEventArchiveV1Exception =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.Id),
                values: "Required");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.Content),
                values: "Required");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.Type),
                values: "Value is not recognized");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.CreatedDate),
                values: "Required");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.UpdatedDate),
                values: "Required");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.ArchivedDate),
                values: "Required");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.EventAddressId),
                values: "Required");

            var expectedEventArchiveV1ValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV1Exception);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(invalidEventArchiveV1);

            EventArchiveV1ValidationException actualEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfArchiveDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfterNow)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventArchiveV1 randomEventArchiveV1 =
                CreateRandomEventArchiveV1(
                    date: randomDateTimeOffset.AddMinutes(minutesBeforeAndAfterNow));

            EventArchiveV1 invalidEventArchiveV1 = randomEventArchiveV1;

            var invalidEventArchiveV1Exception =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.ArchivedDate),
                values: "Date is not recent");

            var expectedEventArchiveV1ValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(invalidEventArchiveV1);

            EventArchiveV1ValidationException actualEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
