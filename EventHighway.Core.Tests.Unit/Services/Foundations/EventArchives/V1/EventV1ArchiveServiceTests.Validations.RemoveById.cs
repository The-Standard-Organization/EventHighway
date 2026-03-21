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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1ArchiveId = Guid.Empty;

            var invalidEventV1ArchiveException =
                new InvalidEventV1ArchiveException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive.Id),
                values: "Required");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            // when
            ValueTask<EventV1Archive> removeEventV1ArchiveByIdTask =
                this.eventV1ArchiveService.RemoveEventV1ArchiveByIdAsync(
                    invalidEventV1ArchiveId);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    removeEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ArchiveByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventV1ArchiveIsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingEventV1ArchiveId = GetRandomId();
            EventV1Archive nullEventV1Archive = null;

            var notFoundEventV1ArchiveException =
                new NotFoundEventV1ArchiveException(
                    message: $"Could not find event archive with id: {nonExistingEventV1ArchiveId}.");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventV1ArchiveException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ArchiveByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(nullEventV1Archive);

            // when
            ValueTask<EventV1Archive> removeEventV1ArchiveByIdTask =
                this.eventV1ArchiveService.RemoveEventV1ArchiveByIdAsync(nonExistingEventV1ArchiveId);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    removeEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ArchiveByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
