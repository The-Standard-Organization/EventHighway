// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAddressUsageReportV2Async()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, inputWindowStart, cancelledToken);

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

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAddressUsageReportV2IfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutHealthV2CoordinationException =
                new TimeoutHealthV2CoordinationException(
                    message: "Health coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedHealthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health coordination dependency error occurred, contact support.",
                    innerException: timeoutHealthV2CoordinationException);

            this.healthInfrastructureV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthV2CoordinationDependencyException actualHealthV2CoordinationDependencyException =
                await Assert.ThrowsAsync<HealthV2CoordinationDependencyException>(
                    retrieveHealthReportTask.AsTask);

            // then
            actualHealthV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationDependencyException);

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationDependencyException))),
                        Times.Once);

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAddressUsageReportV2IfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);

            var expectedHealthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health coordination dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.healthInfrastructureV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthV2CoordinationDependencyException actualHealthV2CoordinationDependencyException =
                await Assert.ThrowsAsync<HealthV2CoordinationDependencyException>(
                    retrieveHealthReportTask.AsTask);

            // then
            actualHealthV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationDependencyException);

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationDependencyException))),
                        Times.Once);

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveAddressUsageReportV2IfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);

            var expectedHealthV2CoordinationDependencyValidationException =
                new HealthV2CoordinationDependencyValidationException(
                    message: "Health coordination validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.healthInfrastructureV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthV2CoordinationDependencyValidationException
                actualHealthV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<HealthV2CoordinationDependencyValidationException>(
                        retrieveHealthReportTask.AsTask);

            // then
            actualHealthV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationDependencyValidationException);

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAddressUsageReportV2IfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomPeriodAlignedWindowStart(inputPeriod);

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedHealthV2CoordinationServiceException =
                new FailedHealthV2CoordinationServiceException(
                    message: "Failed health coordination service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedHealthV2CoordinationServiceException =
                new HealthV2CoordinationServiceException(
                    message: "Health coordination service error occurred, contact support.",
                    innerException: failedHealthV2CoordinationServiceException);

            this.healthInfrastructureV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HealthReportV2> retrieveHealthReportTask =
                this.healthV2CoordinationService.RetrieveAddressUsageReportV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthV2CoordinationServiceException actualHealthV2CoordinationServiceException =
                await Assert.ThrowsAsync<HealthV2CoordinationServiceException>(
                    retrieveHealthReportTask.AsTask);

            // then
            actualHealthV2CoordinationServiceException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationServiceException);

            this.healthInfrastructureV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthReportV2Async(inputPeriod, inputWindowStart, null, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationServiceException))),
                        Times.Once);

            this.healthInfrastructureV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.healthArchivedEventsV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
