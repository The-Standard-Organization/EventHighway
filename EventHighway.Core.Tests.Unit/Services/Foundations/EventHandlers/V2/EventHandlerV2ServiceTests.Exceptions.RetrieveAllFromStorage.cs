// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllFromStorageIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventHandlerV2Exception =
                new FailedStorageEventHandlerV2Exception(
                    message: "Failed event handler storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventHandlerV2DependencyException =
                new EventHandlerV2DependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: failedStorageEventHandlerV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sFromStorageTask =
                this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    randomCancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    retrieveAllEventHandlerV2sFromStorageTask.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllFromStorageIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventHandlerV2Exception =
                new TimeoutEventHandlerV2Exception(
                    message: "Failed event handler timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventHandlerV2DependencyException =
                new EventHandlerV2DependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: timeoutEventHandlerV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sFromStorageTask =
                this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    TestContext.Current.CancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    retrieveAllEventHandlerV2sFromStorageTask.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllFromStorageIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventHandlerV2ServiceException =
                new FailedEventHandlerV2ServiceException(
                    message: "Failed event handler service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventHandlerV2ServiceException =
                new EventHandlerV2ServiceException(
                    message: "Event handler service error occurred, contact support.",
                    innerException: failedEventHandlerV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sFromStorageTask =
                this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    randomCancellationToken);

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ServiceException>(
                    retrieveAllEventHandlerV2sFromStorageTask.AsTask);

            // then
            actualEventHandlerV2ServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ServiceException))),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllFromStorageAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sFromStorageTask =
                this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllEventHandlerV2sFromStorageTask.AsTask);

            actualException.Should().NotBeOfType<EventHandlerV2DependencyException>();
            actualException.Should().NotBeOfType<EventHandlerV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
