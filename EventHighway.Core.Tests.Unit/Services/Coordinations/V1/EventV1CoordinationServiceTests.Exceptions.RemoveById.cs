// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            Guid someEventV1Id = GetRandomId();

            var expectedEventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1CoordinationService.RemoveEventByIdAsync(
                    someEventV1Id);

            EventV1CoordinationDependencyValidationException
                actualEventV1CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyValidationException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyValidationException);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyValidationException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someEventV1Id = GetRandomId();

            var expectedEventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1CoordinationService.RemoveEventByIdAsync(
                    someEventV1Id);

            EventV1CoordinationDependencyException
                actualEventV1CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyException);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventV1Id = GetRandomId();
            var serviceException = new Exception();

            var failedEventV1CoordinationServiceException =
                new FailedEventV1CoordinationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1CoordinationExceptionException =
                new EventV1CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1CoordinationServiceException);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1CoordinationService.RemoveEventByIdAsync(
                    someEventV1Id);

            EventV1CoordinationServiceException
                actualEventV1CoordinationServiceException =
                    await Assert.ThrowsAsync<EventV1CoordinationServiceException>(
                        addEventV1Task.AsTask);

            // then
            actualEventV1CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationExceptionException);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationExceptionException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
