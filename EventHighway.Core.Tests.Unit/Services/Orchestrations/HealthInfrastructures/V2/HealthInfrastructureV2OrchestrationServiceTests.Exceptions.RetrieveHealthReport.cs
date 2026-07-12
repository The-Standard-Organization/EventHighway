// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthInfrastructures.V2
{
    public partial class HealthInfrastructureV2OrchestrationServiceTests
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
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), null, cancelledToken);

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

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
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

            var timeoutHealthInfrastructureV2OrchestrationException =
                new TimeoutHealthInfrastructureV2OrchestrationException(
                    message: "Health infrastructure orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedHealthInfrastructureV2OrchestrationDependencyException =
                new HealthInfrastructureV2OrchestrationDependencyException(
                    message: "Health infrastructure dependency error occurred, contact support.",
                    innerException: timeoutHealthInfrastructureV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), null, randomCancellationToken);

            HealthInfrastructureV2OrchestrationDependencyException
                actualHealthInfrastructureV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthInfrastructureV2OrchestrationDependencyException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthInfrastructureV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedHealthInfrastructureV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthInfrastructureV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
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

            var expectedHealthInfrastructureV2OrchestrationDependencyException =
                new HealthInfrastructureV2OrchestrationDependencyException(
                    message: "Health infrastructure dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), null, randomCancellationToken);

            HealthInfrastructureV2OrchestrationDependencyException
                actualHealthInfrastructureV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthInfrastructureV2OrchestrationDependencyException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthInfrastructureV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedHealthInfrastructureV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthInfrastructureV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
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

            var expectedHealthInfrastructureV2OrchestrationDependencyValidationException =
                new HealthInfrastructureV2OrchestrationDependencyValidationException(
                    message: "Health infrastructure validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), null, randomCancellationToken);

            HealthInfrastructureV2OrchestrationDependencyValidationException
                actualHealthInfrastructureV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<HealthInfrastructureV2OrchestrationDependencyValidationException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthInfrastructureV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHealthInfrastructureV2OrchestrationDependencyValidationException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthInfrastructureV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
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

            var failedHealthInfrastructureV2OrchestrationServiceException =
                new FailedHealthInfrastructureV2OrchestrationServiceException(
                    message: "Failed health infrastructure service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedHealthInfrastructureV2OrchestrationServiceException =
                new HealthInfrastructureV2OrchestrationServiceException(
                    message: "Health infrastructure service error occurred, contact support.",
                    innerException: failedHealthInfrastructureV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), null, randomCancellationToken);

            HealthInfrastructureV2OrchestrationServiceException
                actualHealthInfrastructureV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<HealthInfrastructureV2OrchestrationServiceException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthInfrastructureV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedHealthInfrastructureV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthInfrastructureV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
