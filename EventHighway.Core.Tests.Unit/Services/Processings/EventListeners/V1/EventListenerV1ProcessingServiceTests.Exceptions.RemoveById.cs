// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V1
{
    public partial class EventListenerV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();

            var expectedEventListenerV1ProcessingDependencyValidationException =
                new EventListenerV1ProcessingDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV1ServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1ProcessingService.RemoveEventListenerV1ByIdAsync(
                    someEventListenerV1Id);

            EventListenerV1ProcessingDependencyValidationException
                actualEventListenerV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV1ProcessingDependencyValidationException>(
                        removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1ProcessingDependencyValidationException);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();

            var expectedEventListenerV1ProcessingDependencyException =
                new EventListenerV1ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV1ServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1ProcessingService.RemoveEventListenerV1ByIdAsync(
                    someEventListenerV1Id);

            EventListenerV1ProcessingDependencyException
                actualEventListenerV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventListenerV1ProcessingDependencyException>(
                        removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1ProcessingDependencyException);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV1ProcessingServiceException =
                new FailedEventListenerV1ProcessingServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV1ProcessingExceptionException =
                new EventListenerV1ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV1ProcessingServiceException);

            this.eventListenerV1ServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1ProcessingService.RemoveEventListenerV1ByIdAsync(
                    someEventListenerV1Id);

            EventListenerV1ProcessingServiceException
                actualEventListenerV1ProcessingServiceException =
                    await Assert.ThrowsAsync<EventListenerV1ProcessingServiceException>(
                        addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1ProcessingExceptionException);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingExceptionException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
