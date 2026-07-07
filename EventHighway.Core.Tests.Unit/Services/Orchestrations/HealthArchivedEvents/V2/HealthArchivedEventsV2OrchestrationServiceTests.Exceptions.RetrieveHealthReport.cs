// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.HealthArchivedEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthArchivedEvents.V2
{
    public partial class HealthArchivedEventsV2OrchestrationServiceTests
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
                this.healthArchivedEventsV2OrchestrationService
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

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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

            var timeoutHealthArchivedEventsV2OrchestrationException =
                new TimeoutHealthArchivedEventsV2OrchestrationException(
                    message: "Health archived events orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedHealthArchivedEventsV2OrchestrationDependencyException =
                new HealthArchivedEventsV2OrchestrationDependencyException(
                    message: "Health archived events dependency error occurred, contact support.",
                    innerException: timeoutHealthArchivedEventsV2OrchestrationException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthArchivedEventsV2OrchestrationDependencyException
                actualHealthArchivedEventsV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthArchivedEventsV2OrchestrationDependencyException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthArchivedEventsV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedHealthArchivedEventsV2OrchestrationDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthArchivedEventsV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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

            var expectedHealthArchivedEventsV2OrchestrationDependencyException =
                new HealthArchivedEventsV2OrchestrationDependencyException(
                    message: "Health archived events dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthArchivedEventsV2OrchestrationDependencyException
                actualHealthArchivedEventsV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthArchivedEventsV2OrchestrationDependencyException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthArchivedEventsV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedHealthArchivedEventsV2OrchestrationDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthArchivedEventsV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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

            var expectedHealthArchivedEventsV2OrchestrationDependencyValidationException =
                new HealthArchivedEventsV2OrchestrationDependencyValidationException(
                    message: "Health archived events validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthArchivedEventsV2OrchestrationDependencyValidationException
                actualHealthArchivedEventsV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<HealthArchivedEventsV2OrchestrationDependencyValidationException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthArchivedEventsV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHealthArchivedEventsV2OrchestrationDependencyValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthArchivedEventsV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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

            var failedHealthArchivedEventsV2OrchestrationServiceException =
                new FailedHealthArchivedEventsV2OrchestrationServiceException(
                    message: "Failed health archived events service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedHealthArchivedEventsV2OrchestrationServiceException =
                new HealthArchivedEventsV2OrchestrationServiceException(
                    message: "Health archived events service error occurred, contact support.",
                    innerException: failedHealthArchivedEventsV2OrchestrationServiceException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            HealthArchivedEventsV2OrchestrationServiceException
                actualHealthArchivedEventsV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<HealthArchivedEventsV2OrchestrationServiceException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthArchivedEventsV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedHealthArchivedEventsV2OrchestrationServiceException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthArchivedEventsV2OrchestrationServiceException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
