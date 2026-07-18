// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IReadOnlyList<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(new EventHandlerV2Query(), 
                    randomCancellationToken);

            EventHandlerV2ProcessingDependencyException
                actualEventHandlerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyException>(
                        retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByQueryIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IReadOnlyList<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(new EventHandlerV2Query(), 
                    randomCancellationToken);

            EventHandlerV2ProcessingServiceException
                actualEventHandlerV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingServiceException>(
                        retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualEventHandlerV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingServiceException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingServiceException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfTimeoutOccursAndLogItAsync()
        {
            // given
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
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(new EventHandlerV2Query(), 
                    TestContext.Current.CancellationToken);

            EventHandlerV2ProcessingDependencyException
                actualEventHandlerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingDependencyException>(
                        retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualEventHandlerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ProcessingDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
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
            ValueTask<IReadOnlyList<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(new EventHandlerV2Query(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllEventHandlerV2sTask.AsTask);

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
