// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRetrieveByEventAddressIdIfValidationErrorAndLogItAsync()
        {
            // given
            Guid someEventAddressId = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV1ProcessingValidationException =
                new EventListenerV1ProcessingValidationException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV1OrchestrationDependencyValidationException =
                new EventListenerV1OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: eventListenerV1ProcessingValidationException.InnerException as Xeption);

            this.eventListenerV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(someEventAddressId))
                    .ThrowsAsync(eventListenerV1ProcessingValidationException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1OrchestrationService.RetrieveEventListenerV1sByEventAddressIdAsync(
                    someEventAddressId);

            EventListenerV1OrchestrationDependencyValidationException
                actualEventListenerV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationDependencyValidationException>(
                        retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationDependencyValidationException);

            this.eventListenerV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(someEventAddressId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventListenerV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfDependencyErrorOccursAndLogItAsync(
            Xeption eventListenerV1DependencyException)
        {
            // given
            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: eventListenerV1DependencyException.InnerException as Xeption);

            this.eventListenerV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(someEventAddressId))
                    .ThrowsAsync(eventListenerV1DependencyException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1OrchestrationService.RetrieveEventListenerV1sByEventAddressIdAsync(
                    someEventAddressId);

            EventListenerV1OrchestrationDependencyException actualEventListenerV1OrchestrationDependencyException =
                await Assert.ThrowsAsync<EventListenerV1OrchestrationDependencyException>(
                    retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationDependencyException);

            this.eventListenerV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(someEventAddressId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationDependencyException))),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByEventAddressIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressId = GetRandomId();
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

            this.eventListenerV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(someEventAddressId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1OrchestrationService.RetrieveEventListenerV1sByEventAddressIdAsync(
                    someEventAddressId);

            EventListenerV1OrchestrationServiceException actualEventListenerV1OrchestrationServiceException =
                await Assert.ThrowsAsync<EventListenerV1OrchestrationServiceException>(
                    retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationServiceException);

            this.eventListenerV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(someEventAddressId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationServiceException))),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
