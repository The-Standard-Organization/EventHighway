// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Abstractions.EventHandlers;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

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
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    addEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventHandlerV2AlreadyExistsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            IEventHandler someEventHandler = CreateRandomEventHandler();

            var duplicateKeyException = new DuplicateKeyException(someMessage);
            duplicateKeyException.Data.Add("ErrorCode", new List<string> { "DuplicateKeyError" });

            var alreadyExistsEventHandlerV2Exception =
                new AlreadyExistsEventHandlerV2Exception(
                    message: "Event handler with the same id or name already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventHandlerV2DependencyValidationException =
                new EventHandlerV2DependencyValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventHandlerV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2DependencyValidationException actualEventHandlerV2DependencyValidationException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyValidationException>(
                    addEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2DependencyValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyValidationException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

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
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    addEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfTimeoutOccursAndLogItAsync()
        {
            // given
            IEventHandler someEventHandler = CreateRandomEventHandler();
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
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    someEventHandler, TestContext.Current.CancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    addEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

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
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ServiceException>(
                    addEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventHandlerV2Async(
                    It.IsAny<EventHandlerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ServiceException))),
                            Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnAddAsync()
        {
            // given
            IEventHandler someEventHandler = CreateRandomEventHandler();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEventHandler> addEventHandlerV2Task =
                this.eventHandlerV2Service.AddEventHandlerV2Async(
                    someEventHandler, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    addEventHandlerV2Task.AsTask);

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
