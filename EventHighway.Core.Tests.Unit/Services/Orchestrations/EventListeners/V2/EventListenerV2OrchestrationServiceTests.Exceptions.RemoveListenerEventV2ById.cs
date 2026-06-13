// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(ListenerEventV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveListenerEventV2ByIdIfValidationErrorAndLogItAsync(
            Xeption listenerEventV2ValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();

            var expectedEventListenerV2OrchestrationDependencyValidationException =
                new EventListenerV2OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: listenerEventV2ValidationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(listenerEventV2ValidationException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.eventListenerV2OrchestrationService.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyValidationException
                actualEventListenerV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyValidationException>(
                        removeListenerEventV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ListenerEventV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveListenerEventV2ByIdIfDependencyErrorOccursAndLogItAsync(
            Xeption listenerEventV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();

            var expectedEventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: listenerEventV2DependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(listenerEventV2DependencyException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.eventListenerV2OrchestrationService.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyException
                actualEventListenerV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyException>(
                        removeListenerEventV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveListenerEventV2ByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();
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

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.eventListenerV2OrchestrationService.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            EventListenerV2OrchestrationServiceException
                actualEventListenerV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationServiceException>(
                        removeListenerEventV2ByIdTask.AsTask);

            // then
            actualEventListenerV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
