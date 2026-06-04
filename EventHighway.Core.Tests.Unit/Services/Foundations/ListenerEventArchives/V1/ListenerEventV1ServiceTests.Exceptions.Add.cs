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
    public partial class ListenerEventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();
            SqlException sqlException = GetSqlException();

            var failedStorageListenerEventArchiveV1Exception =
                new FailedStorageListenerEventArchiveV1Exception(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventArchiveV1DependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventArchiveV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(someListenerEventArchiveV1);

            ListenerEventArchiveV1DependencyException actualListenerEventArchiveV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1DependencyException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
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
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsListenerEventArchiveV1Exception =
                new AlreadyExistsListenerEventArchiveV1Exception(
                    message: "Listener event archive with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedListenerEventArchiveV1DependencyValidationException =
                new ListenerEventArchiveV1DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsListenerEventArchiveV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(someListenerEventArchiveV1);

            ListenerEventArchiveV1DependencyValidationException actualListenerEventArchiveV1DependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1DependencyValidationException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();
            var dbUpdateException = new DbUpdateException();

            var failedStorageListenerEventArchiveV1Exception =
                new FailedStorageListenerEventArchiveV1Exception(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedListenerEventArchiveV1DependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventArchiveV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(someListenerEventArchiveV1);

            ListenerEventArchiveV1DependencyException actualListenerEventArchiveV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1DependencyException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();
            var serviceException = new Exception();

            var failedListenerEventArchiveV1ServiceException =
                new FailedListenerEventArchiveV1ServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventArchiveV1ServiceException =
                new ListenerEventArchiveV1ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventArchiveV1ServiceException,
                    data: failedListenerEventArchiveV1ServiceException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(someListenerEventArchiveV1);

            ListenerEventArchiveV1ServiceException actualListenerEventArchiveV1ServiceException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ServiceException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV1ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
