// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByQueryIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SqlException sqlException = CreateSqlException();

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

            var failedStorageEventArchiveV2Exception =
                new FailedStorageEventArchiveV2Exception(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedStorageEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllEventArchiveV2sTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(new EventArchiveV2Query(), randomCancellationToken);

            EventArchiveV2DependencyException actualEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyException>(
                    retrieveAllEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByQueryIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

            var failedEventArchiveV2ServiceException =
                new FailedEventArchiveV2ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2ServiceException =
                new EventArchiveV2ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllEventArchiveV2sTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(new EventArchiveV2Query(), randomCancellationToken);

            EventArchiveV2ServiceException actualEventArchiveV2ServiceException =
                await Assert.ThrowsAsync<EventArchiveV2ServiceException>(
                    retrieveAllEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventArchiveV2Exception =
                new TimeoutEventArchiveV2Exception(
                    message: "Failed event archive timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllEventArchiveV2sTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(new EventArchiveV2Query(), 
                    TestContext.Current.CancellationToken);

            EventArchiveV2DependencyException actualEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyException>(
                    retrieveAllEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2DependencyException.Should().BeEquivalentTo(
                expectedEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByQueryAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllEventArchiveV2sTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(new EventArchiveV2Query(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllEventArchiveV2sTask.AsTask);

            actualException.Should().NotBeOfType<EventArchiveV2DependencyException>();
            actualException.Should().NotBeOfType<EventArchiveV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
