// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventListenerV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var expectedEventListenerV2OrchestrationDependencyValidationException =
                new EventListenerV2OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2OrchestrationService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyValidationException
                actualEventListenerV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyValidationException>(
                        removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyValidationException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventListenerV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var expectedEventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2OrchestrationService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyException
                actualEventListenerV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyException>(
                        removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();
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
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2OrchestrationService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            EventListenerV2OrchestrationServiceException
                actualEventListenerV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationServiceException>(
                        removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationServiceException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationServiceException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
