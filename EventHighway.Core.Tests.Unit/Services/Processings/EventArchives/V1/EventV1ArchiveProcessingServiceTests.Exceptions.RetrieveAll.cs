// ---------------------------------------------------------------
// Copyright (c) Aspen Publishing. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Linq;
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync(
            Xeption eventV1ArchiveDependencyException)
        {
            // given
            var expectedEventV1ArchiveProcessingDependencyException =
                new EventV1ArchiveProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: eventV1ArchiveDependencyException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RetrieveAllEventV1ArchivesAsync())
                    .ThrowsAsync(eventV1ArchiveDependencyException);

            // when
            ValueTask<IQueryable<EventV1Archive>> retrieveAllEventV1ArchivesTask =
                this.eventV1ArchiveProcessingService.RetrieveAllEventV1ArchivesAsync();

            EventV1ArchiveProcessingDependencyException actualEventV1ArchiveProcessingDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveProcessingDependencyException>(
                    retrieveAllEventV1ArchivesTask.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyException);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RetrieveAllEventV1ArchivesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
