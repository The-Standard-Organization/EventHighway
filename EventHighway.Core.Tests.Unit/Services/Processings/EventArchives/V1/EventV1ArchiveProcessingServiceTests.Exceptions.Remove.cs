// ---------------------------------------------------------------
// Copyright (c) Aspen Publishing. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRemoveIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset someCutOffDate = GetRandomDateTimeOffset();

            var expectedEventV1ArchiveProcessingDependencyValidationException =
                new EventV1ArchiveProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RetrieveAllEventV1ArchivesAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask removeEventV1ArchivesTask =
                this.eventV1ArchiveProcessingService.RemoveEventV1ArchivesAsync(someCutOffDate);

            EventV1ArchiveProcessingDependencyValidationException
                actualEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveProcessingDependencyValidationException>(
                        removeEventV1ArchivesTask.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyValidationException);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RetrieveAllEventV1ArchivesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RemoveEventV1ArchiveByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
