// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V1
{
    public partial class ListenerEventV1sClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();

            var expectedListenerEventV1ClientDependencyValidationException =
                new ListenerEventV1ClientDependencyValidationException(
                    message: "Listener event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1sClient.RemoveListenerEventV1ByIdAsync(someListenerEventV1Id);

            ListenerEventV1ClientDependencyValidationException
                actualListenerEventV1ClientDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV1ClientDependencyValidationException>(
                        removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ClientDependencyValidationException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV1ClientDependencyException =
                new ListenerEventV1ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: eventListenerV1OrchestrationDependencyException
                        .InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(eventListenerV1OrchestrationDependencyException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1sClient.RemoveListenerEventV1ByIdAsync(someListenerEventV1Id);

            ListenerEventV1ClientDependencyException actualListenerEventV1ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV1ClientDependencyException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ClientDependencyException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV1OrchestrationServiceException =
                new EventListenerV1OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV1ClientServiceException =
                new ListenerEventV1ClientServiceException(
                    message: "Listener event client service error occurred, contact support.",

                    innerException: eventListenerV1OrchestrationServiceException
                        .InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(eventListenerV1OrchestrationServiceException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1sClient.RemoveListenerEventV1ByIdAsync(someListenerEventV1Id);

            ListenerEventV1ClientServiceException actualListenerEventV1ClientServiceException =
                await Assert.ThrowsAsync<ListenerEventV1ClientServiceException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ClientServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ClientServiceException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
