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
        public async Task ShouldThrowServiceExceptionOnRegisterIfExceptionOccursAndLogItAsync()
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

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.Register(It.IsAny<IEventHandler>()))
                    .Throws(serviceException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Service.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ServiceException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ServiceException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventHandlerV2ServiceException))),
                            Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRegisterAsync()
        {
            // given
            IEventHandler someEventHandler = CreateRandomEventHandler();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Service.RegisterEventHandlerV2Async(
                    someEventHandler, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    registerEventHandlerV2Task.AsTask);

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
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
