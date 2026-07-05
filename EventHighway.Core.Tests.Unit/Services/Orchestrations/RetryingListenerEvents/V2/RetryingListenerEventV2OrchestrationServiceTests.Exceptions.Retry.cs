// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetryingListenerEvents.V2
{
    public partial class RetryingListenerEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetryIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListenerV2.PromotedProperties = null;

            var expectedRetryingListenerEventV2OrchestrationDependencyValidationException =
                new RetryingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            RetryingListenerEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationDependencyValidationException>(
                    retryTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnRetryIfDependencyErrorOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListenerV2.PromotedProperties = null;

            var expectedRetryingListenerEventV2OrchestrationDependencyException =
                new RetryingListenerEventV2OrchestrationDependencyException(
                    message: "Retrying listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(dependencyException);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            RetryingListenerEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationDependencyException>(
                    retryTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetryIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListenerV2.PromotedProperties = null;

            var serviceException = new Exception();

            var failedRetryingListenerEventV2OrchestrationServiceException =
                new FailedRetryingListenerEventV2OrchestrationServiceException(
                    message: "Failed retrying listener event orchestration service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedRetryingListenerEventV2OrchestrationServiceException =
                new RetryingListenerEventV2OrchestrationServiceException(
                    message: "Retrying listener event service error occurred, contact support.",
                    innerException: failedRetryingListenerEventV2OrchestrationServiceException);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            RetryingListenerEventV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationServiceException>(
                    retryTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetryIfTimeoutOccursAndLogItAsync()
        {
            // given
            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListenerV2.PromotedProperties = null;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutRetryingListenerEventV2OrchestrationException =
                new TimeoutRetryingListenerEventV2OrchestrationException(
                    message: "Failed retrying listener event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedRetryingListenerEventV2OrchestrationDependencyException =
                new RetryingListenerEventV2OrchestrationDependencyException(
                    message: "Retrying listener event dependency error occurred, contact support.",
                    innerException: timeoutRetryingListenerEventV2OrchestrationException);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        TestContext.Current.CancellationToken);

            RetryingListenerEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationDependencyException>(
                    retryTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetryAsync()
        {
            // given
            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retryTask.AsTask);

            actualException.Should()
                .NotBeOfType<RetryingListenerEventV2OrchestrationDependencyException>();

            actualException.Should()
                .NotBeOfType<RetryingListenerEventV2OrchestrationServiceException>();

            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationOccursDuringDispatchOnRetryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListenerV2.PromotedProperties = null;
            someListenerEventV2.RetryAttemptsAllowed = 15;
            someListenerEventV2.RemainingRetryAttempts = 10;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var retryConfiguration = new RetryConfiguration
            {
                RetryAttemptsAllowed = 15,
                RetryBackoffMaxMinutes = 180,
                DeadAfterMinutes = 180
            };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            var operationCanceledException =
                new OperationCanceledException(cancellationTokenSource.Token);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken))
                .ThrowsAsync(operationCanceledException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                .Returns(retryConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ReturnsAsync(someListenerEventV2);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retryTask.AsTask);

            actualException.Should()
                .NotBeOfType<RetryingListenerEventV2OrchestrationDependencyException>();

            actualException.Should()
                .NotBeOfType<RetryingListenerEventV2OrchestrationServiceException>();

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Exception>()),
                    Times.Never);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
