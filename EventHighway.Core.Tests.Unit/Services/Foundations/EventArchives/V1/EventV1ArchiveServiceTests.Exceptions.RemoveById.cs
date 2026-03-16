// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventV1ArchiveId = GetRandomId();
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
                broker.SelectEventV1ArchiveByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventV1Archive> removeEventV1ArchiveByIdTask =
                this.eventV1ArchiveService.RemoveEventV1ArchiveByIdAsync(
                    someEventV1ArchiveId);

            EventV1ArchiveDependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyException>(
                    removeEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ArchiveByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationErrorOnRemoveByIdIfDbUpdateConcurrencyErrorAndLogItAsync()
        {
            // given
            Guid someEventV1ArchiveId = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedEventV1ArchiveException =
                new LockedEventV1ArchiveException(
                    message: "Event archive is locked, try again.",
                    innerException: dbUpdateConcurrencyException);

            var expectedEventV1ArchiveDependencyValidationException =
                new EventV1ArchiveDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: lockedEventV1ArchiveException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ArchiveByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventV1Archive> removeEventV1ArchiveByIdTask =
                this.eventV1ArchiveService.RemoveEventV1ArchiveByIdAsync(
                    someEventV1ArchiveId);

            EventV1ArchiveDependencyValidationException actualEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyValidationException>(
                    removeEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ArchiveByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventV1ArchiveId = GetRandomId();
            var dbUpdateException = new DbUpdateException();

            var failedEventV1ArchiveStorageException =
                new FailedEventV1ArchiveStorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedEventV1ArchiveDependencyException =
                new EventV1ArchiveDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventV1ArchiveStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ArchiveByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventV1Archive> removeEventV1ArchiveByIdTask =
                this.eventV1ArchiveService.RemoveEventV1ArchiveByIdAsync(someEventV1ArchiveId);

            EventV1ArchiveDependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventV1ArchiveDependencyException>(
                    removeEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ArchiveByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
