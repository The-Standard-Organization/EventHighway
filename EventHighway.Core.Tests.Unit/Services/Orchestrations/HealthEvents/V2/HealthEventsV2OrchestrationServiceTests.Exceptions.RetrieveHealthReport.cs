// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.HealthEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthEvents.V2
{
    public partial class HealthEventsV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveHealthReportV2Async()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveHealthReportTask.AsTask);

            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthReportV2IfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutHealthEventsV2OrchestrationException =
                new TimeoutHealthEventsV2OrchestrationException(
                    message: "Health events orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedHealthEventsV2OrchestrationDependencyException =
                new HealthEventsV2OrchestrationDependencyException(
                    message: "Health events dependency error occurred, contact support.",
                    innerException: timeoutHealthEventsV2OrchestrationException);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthEventsV2OrchestrationDependencyException
                actualHealthEventsV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthEventsV2OrchestrationDependencyException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthEventsV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedHealthEventsV2OrchestrationDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthEventsV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthReportV2IfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthEventsV2OrchestrationDependencyException =
                new HealthEventsV2OrchestrationDependencyException(
                    message: "Health events dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthEventsV2OrchestrationDependencyException
                actualHealthEventsV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthEventsV2OrchestrationDependencyException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthEventsV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedHealthEventsV2OrchestrationDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthEventsV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveHealthReportV2IfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthEventsV2OrchestrationDependencyValidationException =
                new HealthEventsV2OrchestrationDependencyValidationException(
                    message: "Health events validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthEventsV2OrchestrationDependencyValidationException
                actualHealthEventsV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<HealthEventsV2OrchestrationDependencyValidationException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthEventsV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHealthEventsV2OrchestrationDependencyValidationException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthEventsV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveHealthReportV2IfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedHealthEventsV2OrchestrationServiceException =
                new FailedHealthEventsV2OrchestrationServiceException(
                    message: "Failed health events service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedHealthEventsV2OrchestrationServiceException =
                new HealthEventsV2OrchestrationServiceException(
                    message: "Health events service error occurred, contact support.",
                    innerException: failedHealthEventsV2OrchestrationServiceException);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthEventsV2OrchestrationServiceException
                actualHealthEventsV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<HealthEventsV2OrchestrationServiceException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthEventsV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedHealthEventsV2OrchestrationServiceException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthEventsV2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
