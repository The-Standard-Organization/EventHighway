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
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            EventV1Archive nullEventV1Archive = null;

            var nullEventV1ArchiveException =
                new NullEventV1ArchiveException(
                    message: "Event archive is null.");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ArchiveException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(nullEventV1Archive);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsInvalidAndLogItAsync(
            string invalidText)
        {
            var invalidEventV1Archive = new EventV1Archive
            {
                Id = Guid.Empty,
                Name = invalidText,
                Description = invalidText,
            };

            var invalidEventV1ArchiveException =
                new InvalidEventV1ArchiveException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.Id),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.Name),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.Description),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.CreatedDate),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.UpdatedDate),
                values: "Required");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(invalidEventV1Archive);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
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
            EventV1Archive randomEventV1Archive = CreateRandomEventV1Archive(dates: randomDateTimeOffset);
            EventV1Archive invalidEventV1Archive = randomEventV1Archive;
            invalidEventV1Archive.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventV1ArchiveException =
                new InvalidEventV1ArchiveException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.CreatedDate),
                values: $"Date is not the same as {nameof(EventV1Archive.UpdatedDate)}");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(invalidEventV1Archive);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
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

            EventV1Archive randomEventV1Archive =
                CreateRandomEventV1Archive(
                    dates: randomDateTimeOffset.AddMinutes(minutesBeforeAndAfterNow));

            EventV1Archive invalidEventV1Archive = randomEventV1Archive;

            var invalidEventV1ArchiveException =
                new InvalidEventV1ArchiveException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.CreatedDate),
                values: "Date is not recent");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(invalidEventV1Archive);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
