// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            Guid someEventV1Id = GetRandomId();

            var expectedEventV1OrchestrationDependencyValidationException =
                new EventV1OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1OrchestrationService.RemoveEventV1ByIdAsync(
                    someEventV1Id);

            EventV1OrchestrationDependencyValidationException
                actualEventV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationDependencyValidationException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyValidationException);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someEventV1Id = GetRandomId();

            var expectedEventV1OrchestrationDependencyException =
                new EventV1OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1OrchestrationService.RemoveEventV1ByIdAsync(
                    someEventV1Id);

            EventV1OrchestrationDependencyException
                actualEventV1OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventV1OrchestrationDependencyException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyException);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventV1Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV1OrchestrationServiceException =
                new FailedEventV1OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV1OrchestrationExceptionException =
                new EventV1OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1OrchestrationServiceException);

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1OrchestrationService.RemoveEventV1ByIdAsync(
                    someEventV1Id);

            EventV1OrchestrationServiceException
                actualEventV1OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventV1OrchestrationServiceException>(
                        addEventV1Task.AsTask);

            // then
            actualEventV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationExceptionException);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationExceptionException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
