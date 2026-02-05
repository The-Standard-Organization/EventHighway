// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            SqlException sqlException = GetSqlException();

            var failedListenerEventV1ArchiveStorageException =
                new FailedListenerEventV1ArchiveStorageException(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedListenerEventV1ArchiveDependencyException =
                new ListenerEventV1ArchiveDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventV1ArchiveDependencyException actualListenerEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveDependencyException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsListenerEventV1ArchiveException =
                new AlreadyExistsListenerEventV1ArchiveException(
                    message: "Listener event archive with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedListenerEventV1ArchiveDependencyValidationException =
                new ListenerEventV1ArchiveDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsListenerEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventV1ArchiveDependencyValidationException actualListenerEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveDependencyValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var dbUpdateException = new DbUpdateException();

            var failedListenerEventV1ArchiveStorageException =
                new FailedListenerEventV1ArchiveStorageException(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedListenerEventV1ArchiveDependencyException =
                new ListenerEventV1ArchiveDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventV1ArchiveDependencyException actualListenerEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveDependencyException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var serviceException = new Exception();

            var failedListenerEventV1ArchiveServiceException =
                new FailedListenerEventV1ArchiveServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedListenerEventV1ArchiveServiceException =
                new ListenerEventV1ArchiveServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventV1ArchiveServiceException actualListenerEventV1ArchiveServiceException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveServiceException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
