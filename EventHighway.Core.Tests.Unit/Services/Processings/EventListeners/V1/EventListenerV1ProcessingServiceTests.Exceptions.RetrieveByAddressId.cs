// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV1ProcessingDependencyException =
                new EventListenerV1ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV1ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV1sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1ProcessingService.RetrieveEventListenerV1sByEventAddressIdAsync(
                    someEventAddressId);

            EventListenerV1ProcessingDependencyException actualEventListenerV1ProcessingDependencyException =
                await Assert.ThrowsAsync<EventListenerV1ProcessingDependencyException>(
                    retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1ProcessingDependencyException);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnOnRetrieveByEventAddressIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressId = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV1ProcessingServiceException =
                new FailedEventListenerV1ProcessingServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV1ProcessingServiceException =
                new EventListenerV1ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV1ProcessingServiceException);

            this.eventListenerV1ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1ProcessingService.RetrieveEventListenerV1sByEventAddressIdAsync(
                    someEventAddressId);

            EventListenerV1ProcessingServiceException actualEventListenerV1ProcessingServiceException =
                await Assert.ThrowsAsync<EventListenerV1ProcessingServiceException>(
                    retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1ProcessingServiceException);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingServiceException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
