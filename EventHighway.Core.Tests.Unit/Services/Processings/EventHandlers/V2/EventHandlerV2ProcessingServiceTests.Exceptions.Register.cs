// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRegisterIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

            var expectedEventHandlerV2ProcessingDependencyValidationException =
                new EventHandlerV2ProcessingDependencyValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    randomCancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ProcessingDependencyValidationException
                actualEventHandlerV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyValidationException>(
                        registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyValidationException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRegisterIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

            var expectedEventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    randomCancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ProcessingDependencyException
                actualEventHandlerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyException>(
                        registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRegisterIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventHandlerV2ProcessingServiceException =
                new FailedEventHandlerV2ProcessingServiceException(
                    message: "Failed event handler service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventHandlerV2ProcessingServiceException =
                new EventHandlerV2ProcessingServiceException(
                    message: "Event handler service error occurred, contact support.",
                    innerException: failedEventHandlerV2ProcessingServiceException);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    randomCancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ProcessingServiceException
                actualEventHandlerV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingServiceException>(
                        registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingServiceException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingServiceException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRegisterIfTimeoutOccursAndLogItAsync()
        {
            // given
            IEventHandler someEventHandler = CreateRandomEventHandler();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventHandlerV2ProcessingException =
                new TimeoutEventHandlerV2ProcessingException(
                    message: "Failed event handler processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: timeoutEventHandlerV2ProcessingException);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    someEventHandler, TestContext.Current.CancellationToken);

            EventHandlerV2ProcessingDependencyException
                actualEventHandlerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyException>(
                        registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.AddEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
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
                this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                    someEventHandler, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    registerEventHandlerV2Task.AsTask);

            actualException.Should().NotBeOfType<EventHandlerV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventHandlerV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
