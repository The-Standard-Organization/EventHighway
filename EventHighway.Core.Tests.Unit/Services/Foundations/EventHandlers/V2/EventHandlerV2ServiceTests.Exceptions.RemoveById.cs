// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

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
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Remove(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "DbUpdateError" });

            var failedStorageEventHandlerV2Exception =
                new FailedStorageEventHandlerV2Exception(
                    message: "Failed event handler storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventHandlerV2DependencyException =
                new EventHandlerV2DependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: failedStorageEventHandlerV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Remove(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventHandlerV2Id = GetRandomId();
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
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, TestContext.Current.CancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Remove(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

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
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ServiceException>(
                    removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ServiceException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Remove(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            Guid someEventHandlerV2Id = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventHandlerV2ByIdTask.AsTask);

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
