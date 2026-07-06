// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveInfrastructureHealthV2Async()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<InfrastructureHealthV2> retrieveInfrastructureHealthTask =
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveInfrastructureHealthV2Async(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveInfrastructureHealthTask.AsTask);

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
        public async Task ShouldThrowDependencyExceptionOnRetrieveInfrastructureHealthV2IfTimeoutOccursAndLogItAsync()
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
            ValueTask<InfrastructureHealthV2> retrieveInfrastructureHealthTask =
                this.healthInfrastructureV2OrchestrationService
                    .RetrieveInfrastructureHealthV2Async(randomCancellationToken);

            HealthInfrastructureV2OrchestrationDependencyException
                actualHealthInfrastructureV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<HealthInfrastructureV2OrchestrationDependencyException>(
                        retrieveInfrastructureHealthTask.AsTask);

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
    }
}
