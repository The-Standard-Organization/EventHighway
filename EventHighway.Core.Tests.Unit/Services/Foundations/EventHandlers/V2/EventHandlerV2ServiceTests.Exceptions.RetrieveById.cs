// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfTimeoutOccursAndLogItAsync()
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

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.GetAll())
                    .Throws(operationCanceledException);

            // when
            ValueTask<IEventHandler> retrieveEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RetrieveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, TestContext.Current.CancellationToken);

            EventHandlerV2DependencyException actualEventHandlerV2DependencyException =
                await Assert.ThrowsAsync<EventHandlerV2DependencyException>(
                    retrieveEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2DependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2DependencyException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2DependencyException))),
                            Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
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

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.GetAll())
                    .Throws(serviceException);

            // when
            ValueTask<IEventHandler> retrieveEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RetrieveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ServiceException>(
                    retrieveEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ServiceException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ServiceException))),
                            Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByIdAsync()
        {
            // given
            Guid someEventHandlerV2Id = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEventHandler> retrieveEventHandlerV2ByIdTask =
                this.eventHandlerV2Service.RetrieveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventHandlerV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<EventHandlerV2DependencyException>();
            actualException.Should().NotBeOfType<EventHandlerV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
