// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedEventV1ArchiveStorageException =
                new FailedEventV1ArchiveStorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedEventV1ArchiveDependencyException =
                new EventV1ArchiveDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventV1ArchiveStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventV1ArchivesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventV1Archive>> retrieveAllEventV1ArchivesTask =
                this.eventV1ArchiveService.RetrieveAllEventV1ArchivesAsync();

            EventV1ArchiveDependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyException>(
                    retrieveAllEventV1ArchivesTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV1ArchivesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
