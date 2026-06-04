// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(ListenerEventV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveListenerEventV1IfDependencyValidationAndLogItAsync(
            Xeption listenerEventV1ValidationException)
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();

            var expectedEventListenerV1OrchestrationDependencyValidationException =
                new EventListenerV1OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: listenerEventV1ValidationException.InnerException as Xeption);

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(listenerEventV1ValidationException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.eventListenerV1OrchestrationService.RemoveListenerEventV1ByIdAsync(
                    someListenerEventV1Id);

            EventListenerV1OrchestrationDependencyValidationException
                actualEventListenerV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationDependencyValidationException>(
                        removeListenerEventV1ByIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationDependencyValidationException);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ListenerEventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveListenerEventV1ByIdIfDependencyExceptionAndLogItAsync(
            Xeption listenerEventV1DependencyException)
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();

            var expectedEventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: listenerEventV1DependencyException.InnerException as Xeption);

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(listenerEventV1DependencyException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.eventListenerV1OrchestrationService.RemoveListenerEventV1ByIdAsync(
                    someListenerEventV1Id);

            EventListenerV1OrchestrationDependencyException
                actualEventListenerV1OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationDependencyException>(
                        removeListenerEventV1ByIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationDependencyException);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveListenerEventV1ByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV1OrchestrationServiceException =
                new FailedEventListenerV1OrchestrationServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV1OrchestrationServiceException =
                new EventListenerV1OrchestrationServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV1OrchestrationServiceException);

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1Task =
                this.eventListenerV1OrchestrationService.RemoveListenerEventV1ByIdAsync(
                    someListenerEventV1Id);

            EventListenerV1OrchestrationServiceException
                actualEventListenerV1OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationServiceException>(
                        removeListenerEventV1Task.AsTask);

            // then
            actualEventListenerV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationServiceException);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
