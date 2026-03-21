// ---------------------------------------------------------------
// Copyright (c) Aspen Publishing. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventV1ArchiveIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1ArchiveId = Guid.Empty;

            var invalidEventV1ArchiveProcessingException =
                new InvalidEventV1ArchiveProcessingException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveProcessingException.AddData(
                key: nameof(EventV1Archive.Id),
                values: "Required");

            var expectedEventV1ArchiveProcessingValidationException =
                new EventV1ArchiveProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveProcessingException);

            // when
            ValueTask<EventV1Archive> removeEventV1ArchiveByIdTask =
                this.eventV1ArchiveProcessingService.RemoveEventV1ArchiveByIdAsync(invalidEventV1ArchiveId);

            EventV1ArchiveProcessingValidationException actualEventV1ArchiveProcessingValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveProcessingValidationException>(
                    removeEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(
                    It.Is(SameExceptionAs(
                        expectedEventV1ArchiveProcessingValidationException))),
                            Times.Once);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RemoveEventV1ArchiveByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }
    }
}
