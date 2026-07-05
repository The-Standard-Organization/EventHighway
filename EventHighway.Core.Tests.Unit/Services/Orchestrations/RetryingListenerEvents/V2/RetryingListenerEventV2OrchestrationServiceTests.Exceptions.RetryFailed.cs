// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
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
            ShouldThrowDependencyValidationExceptionOnRetryFailedListenerEventV2sIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            var expectedRetryingListenerEventV2OrchestrationDependencyValidationException =
                new RetryingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask retryFailedTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryFailedListenerEventV2sAsync(randomCancellationToken);

            RetryingListenerEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationDependencyValidationException>(
                    retryFailedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationDependencyValidationException))),
                Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnRetryFailedListenerEventV2sIfDependencyErrorOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            var expectedRetryingListenerEventV2OrchestrationDependencyException =
                new RetryingListenerEventV2OrchestrationDependencyException(
                    message: "Retrying listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(dependencyException);

            // when
            ValueTask retryFailedTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryFailedListenerEventV2sAsync(randomCancellationToken);

            RetryingListenerEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationDependencyException>(
                    retryFailedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationDependencyException))),
                Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetryFailedListenerEventV2sIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

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

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(serviceException);

            // when
            ValueTask retryFailedTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryFailedListenerEventV2sAsync(randomCancellationToken);

            RetryingListenerEventV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationServiceException>(
                    retryFailedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationServiceException))),
                Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
