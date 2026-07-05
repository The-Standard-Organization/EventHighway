// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnBulkModifyIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<ListenerEventV2> someListenerEventV2s = CreateRandomRestoreListenerEventV2s();
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageListenerEventV2Exception =
                new FailedStorageListenerEventV2Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkModifyListenerEventV2sTask =
                this.listenerEventV2Service.BulkModifyListenerEventV2sAsync(
                    someListenerEventV2s,
                        randomCancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    bulkModifyListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnBulkModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<ListenerEventV2> someListenerEventV2s = CreateRandomRestoreListenerEventV2s();
            var dbUpdateException = new DbUpdateException();

            var failedStorageListenerEventV2Exception =
                new FailedStorageListenerEventV2Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkModifyListenerEventV2sTask =
                this.listenerEventV2Service.BulkModifyListenerEventV2sAsync(
                    someListenerEventV2s,
                        randomCancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    bulkModifyListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnBulkModifyIfTimeoutOccursAndLogItAsync()
        {
            // given
            List<ListenerEventV2> someListenerEventV2s = CreateRandomRestoreListenerEventV2s();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventV2Exception =
                new TimeoutListenerEventV2Exception(
                    message: "Failed listener event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkModifyListenerEventV2sTask =
                this.listenerEventV2Service.BulkModifyListenerEventV2sAsync(
                    someListenerEventV2s, TestContext.Current.CancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    bulkModifyListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should().BeEquivalentTo(
                expectedListenerEventV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnBulkModifyAsync()
        {
            // given
            List<ListenerEventV2> someListenerEventV2s = CreateRandomRestoreListenerEventV2s();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkModifyListenerEventV2sTask =
                this.listenerEventV2Service.BulkModifyListenerEventV2sAsync(
                    someListenerEventV2s, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    bulkModifyListenerEventV2sTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventV2DependencyException>();
            actualException.Should().NotBeOfType<ListenerEventV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<ListenerEventV2> someListenerEventV2s = CreateRandomRestoreListenerEventV2s();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedListenerEventV2ServiceException =
                new FailedListenerEventV2ServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV2ServiceException =
                new ListenerEventV2ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkModifyListenerEventV2sTask =
                this.listenerEventV2Service.BulkModifyListenerEventV2sAsync(
                    someListenerEventV2s,
                        randomCancellationToken);

            ListenerEventV2ServiceException actualListenerEventV2ServiceException =
                await Assert.ThrowsAsync<ListenerEventV2ServiceException>(
                    bulkModifyListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.IsAny<List<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
