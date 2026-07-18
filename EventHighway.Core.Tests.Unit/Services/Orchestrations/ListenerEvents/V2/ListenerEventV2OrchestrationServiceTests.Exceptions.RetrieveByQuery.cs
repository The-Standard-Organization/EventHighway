// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutListenerEventV2OrchestrationException =
                new TimeoutListenerEventV2OrchestrationException(
                    message: "Failed listener event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedListenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2OrchestrationException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveListenerEventV2sByQueryAsync(new ListenerEventV2Query(), randomCancellationToken);

            ListenerEventV2OrchestrationDependencyException
                actualListenerEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationDependencyException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveByQueryIfValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventV2OrchestrationDependencyValidationException =
                new ListenerEventV2OrchestrationDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveListenerEventV2sByQueryAsync(new ListenerEventV2Query(), randomCancellationToken);

            ListenerEventV2OrchestrationDependencyValidationException
                actualListenerEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationDependencyValidationException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveListenerEventV2sByQueryAsync(new ListenerEventV2Query(), randomCancellationToken);

            ListenerEventV2OrchestrationDependencyException
                actualListenerEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationDependencyException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByQueryIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();

            var failedListenerEventV2OrchestrationServiceException =
                new FailedListenerEventV2OrchestrationServiceException(
                    message: "Failed listener event orchestration service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV2OrchestrationServiceException =
                new ListenerEventV2OrchestrationServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveListenerEventV2sByQueryAsync(new ListenerEventV2Query(), randomCancellationToken);

            ListenerEventV2OrchestrationServiceException
                actualListenerEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationServiceException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByQueryAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveListenerEventV2sByQueryAsync(new ListenerEventV2Query(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<ListenerEventV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
