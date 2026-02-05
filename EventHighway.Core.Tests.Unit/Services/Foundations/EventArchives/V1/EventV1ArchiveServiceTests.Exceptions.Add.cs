// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();
            SqlException sqlException = CreateSqlException();

            var failedEventV1ArchiveStorageException =
                new FailedEventV1ArchiveStorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedEventV1ArchiveDependencyException =
                new EventV1ArchiveDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveDependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventV1ArchiveAlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsEventV1ArchiveException =
                new AlreadyExistsEventV1ArchiveException(
                    message: "Event archive with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedEventV1ArchiveDependencyValidationException =
                new EventV1ArchiveDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveDependencyValidationException actualEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveDependencyValidationException actualEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();
            var dbUpdateException = new DbUpdateException();

            var failedEventV1ArchiveStorageException =
                new FailedEventV1ArchiveStorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedEventV1ArchiveDependencyException =
                new EventV1ArchiveDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveDependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }


    }
}
