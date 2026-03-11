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

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someCutOffDate = GetRandomDateTimeOffset();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            var invalidEventV1ArchiveReferenceException =
                new InvalidEventV1ArchiveReferenceException(
                    message: "Invalid event archive reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedEventV1ArchiveDependencyValidationException =
                new EventV1ArchiveDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveReferenceException);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventV1ArchivesAsync(It.IsAny<DateTimeOffset>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<int> removeEventV1ArchiveTask =
                this.eventV1ArchiveService.RemoveEventV1ArchivesAsync(someCutOffDate);

            EventV1ArchiveDependencyValidationException actualEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyValidationException>(
                    removeEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyValidationException);


            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1ArchivesAsync(
                    It.IsAny<DateTimeOffset>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyValidationException))),
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
