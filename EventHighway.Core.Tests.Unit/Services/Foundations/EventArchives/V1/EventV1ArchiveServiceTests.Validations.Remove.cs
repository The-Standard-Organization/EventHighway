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
        public async Task ShouldThrowValidationExceptionOnRemoveIfArchiveDateIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;
            DateTimeOffset invalidDateTimeOffset = randomDateTimeOffset;

            var invalidEventV1ArchiveException =
                new InvalidEventV1ArchiveException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.CreatedDate),
                values: "Required");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            // when
            ValueTask<int> removeEventV1ArchiveTask =
                this.eventV1ArchiveService.RemoveEventV1ArchivesAsync(invalidDateTimeOffset);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    removeEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1ArchivesAsync(It.IsAny<DateTimeOffset>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
