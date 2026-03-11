// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someCutOffDate = GetRandomDateTimeOffset();
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
                broker.DeleteEventV1ArchivesAsync(It.IsAny<DateTimeOffset>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<int> removeEventV1ArchiveTask =
                this.eventV1ArchiveService.RemoveEventV1ArchivesAsync(someCutOffDate);

            EventV1ArchiveDependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyException>(
                    removeEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1ArchivesAsync(It.IsAny<DateTimeOffset>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
