// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V1
{
    public partial class EventListenerV1sClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();

            var expectedEventListenerV1ClientDependencyValidationException =
                new EventListenerV1ClientDependencyValidationException(
                    message: "Event listener client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1SClient.RemoveEventListenerV1ByIdAsync(someEventListenerV1Id);

            EventListenerV1ClientDependencyValidationException
                actualEventListenerV1ClientDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV1ClientDependencyValidationException>(
                        removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1ClientDependencyValidationException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV1ClientDependencyException =
                new EventListenerV1ClientDependencyException(
                    message: "Event listener client dependency error occurred, contact support.",

                    innerException: eventListenerV1OrchestrationDependencyException
                        .InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(eventListenerV1OrchestrationDependencyException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1SClient.RemoveEventListenerV1ByIdAsync(someEventListenerV1Id);

            EventListenerV1ClientDependencyException
                actualEventListenerV1ClientDependencyException =
                    await Assert.ThrowsAsync<EventListenerV1ClientDependencyException>(
                        removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1ClientDependencyException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV1OrchestrationServiceException =
                new EventListenerV1OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV1ClientServiceException =
                new EventListenerV1ClientServiceException(
                    message: "Event listener client service error occurred, contact support.",

                    innerException: eventListenerV1OrchestrationServiceException
                        .InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(eventListenerV1OrchestrationServiceException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1SClient.RemoveEventListenerV1ByIdAsync(someEventListenerV1Id);

            EventListenerV1ClientServiceException actualEventListenerV1ClientServiceException =
                await Assert.ThrowsAsync<EventListenerV1ClientServiceException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ClientServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1ClientServiceException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
