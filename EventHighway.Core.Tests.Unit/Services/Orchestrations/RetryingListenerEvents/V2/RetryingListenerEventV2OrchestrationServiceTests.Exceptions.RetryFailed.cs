// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
    }
}
