// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRetrieveByEventAddressIdByQueryIfValidationErrorAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV2ProcessingValidationException =
                new EventListenerV2ProcessingValidationException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV2OrchestrationDependencyValidationException =
                new EventListenerV2OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: eventListenerV2ProcessingValidationException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2ProcessingValidationException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyValidationException
                actualEventListenerV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyValidationException>(
                        retrieveTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyValidationException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdByQueryIfDependencyErrorOccursAndLogItAsync(
            Xeption eventListenerV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: eventListenerV2DependencyException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2DependencyException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyException
                actualEventListenerV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyException>(
                        retrieveTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByEventAddressIdByQueryIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV2OrchestrationServiceException =
                new FailedEventListenerV2OrchestrationServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV2OrchestrationServiceException =
                new EventListenerV2OrchestrationServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV2OrchestrationServiceException);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2OrchestrationServiceException
                actualEventListenerV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationServiceException>(
                        retrieveTask.AsTask);

            // then
            actualEventListenerV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationServiceException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationServiceException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdByQueryIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressId = GetRandomId();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutEventListenerV2OrchestrationException =
                new TimeoutEventListenerV2OrchestrationException(
                    message: "Failed event listener orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedEventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: timeoutEventListenerV2OrchestrationException);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    TestContext.Current.CancellationToken);

            EventListenerV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByEventAddressIdByQueryAsync()
        {
            // given
            Guid someEventAddressId = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            actualException.Should().NotBeOfType<EventListenerV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<EventListenerV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
