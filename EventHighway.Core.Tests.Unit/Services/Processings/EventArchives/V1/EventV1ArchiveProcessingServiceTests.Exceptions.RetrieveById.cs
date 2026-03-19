// ---------------------------------------------------------------
// Copyright (c) Aspen Publishing. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            Guid someEventV1ArchiveId = GetRandomId();

            var expectedEventV1ArchiveProcessingDependencyValidationException =
                new EventV1ArchiveProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RetrieveEventV1ArchiveByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1Archive> retrieveEventV1ArchiveByIdTask =
                this.eventV1ArchiveProcessingService.RetrieveEventV1ArchiveByIdAsync(someEventV1ArchiveId);

            EventV1ArchiveProcessingDependencyValidationException
                actualEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveProcessingDependencyValidationException>(
                        retrieveEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyValidationException);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RetrieveEventV1ArchiveByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
