// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V1
{
    public partial class EventV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            Guid someEventV1Id = GetRandomId();

            var expectedEventV1ProcessingDependencyValidationException =
                new EventV1ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1ProcessingService.RemoveEventV1ByIdAsync(
                    someEventV1Id);

            EventV1ProcessingDependencyValidationException
                actualEventV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ProcessingDependencyValidationException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingDependencyValidationException);

            this.eventV1ServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someEventV1Id = GetRandomId();

            var expectedEventV1ProcessingDependencyException =
                new EventV1ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1ServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1ProcessingService.RemoveEventV1ByIdAsync(
                    someEventV1Id);

            EventV1ProcessingDependencyException
                actualEventV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventV1ProcessingDependencyException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingDependencyException);

            this.eventV1ServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingDependencyException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventV1Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV1ProcessingServiceException =
                new FailedEventV1ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV1ProcessingExceptionException =
                new EventV1ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1ProcessingServiceException);

            this.eventV1ServiceMock.Setup(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> removeEventV1ByIdTask =
                this.eventV1ProcessingService.RemoveEventV1ByIdAsync(
                    someEventV1Id);

            EventV1ProcessingServiceException
                actualEventV1ProcessingServiceException =
                    await Assert.ThrowsAsync<EventV1ProcessingServiceException>(
                        removeEventV1ByIdTask.AsTask);

            // then
            actualEventV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingExceptionException);

            this.eventV1ServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingExceptionException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
