// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
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
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

            var expectedEventHandlerV2ProcessingDependencyValidationException =
                new EventHandlerV2ProcessingDependencyValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2ProcessingService.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ProcessingDependencyValidationException
                actualEventHandlerV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyValidationException>(
                        removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyValidationException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
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
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

            var expectedEventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2ProcessingService.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ProcessingDependencyException
                actualEventHandlerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyException>(
                        removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
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
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventHandlerV2Id = GetRandomId();

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
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2ProcessingService.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, randomCancellationToken);

            EventHandlerV2ProcessingServiceException
                actualEventHandlerV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingServiceException>(
                        removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingServiceException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
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
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventHandlerV2Id = GetRandomId();
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
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2ProcessingService.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, TestContext.Current.CancellationToken);

            EventHandlerV2ProcessingDependencyException
                actualEventHandlerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyException>(
                        removeEventHandlerV2ByIdTask.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    It.IsAny<Guid>(),
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
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            Guid someEventHandlerV2Id = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventHandlerV2> removeEventHandlerV2ByIdTask =
                this.eventHandlerV2ProcessingService.RemoveEventHandlerV2ByIdAsync(
                    someEventHandlerV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventHandlerV2ByIdTask.AsTask);

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
