// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(ListenerEventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllListenerEventV1sIfDependencyErrorAndLogItAsync(
            Xeption listenerEventV1DependencyException)
        {
            // given
            var expectedEventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: listenerEventV1DependencyException.InnerException as Xeption);

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV1sAsync())
                    .ThrowsAsync(listenerEventV1DependencyException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.eventListenerV1OrchestrationService.RetrieveAllListenerEventV1sAsync();

            EventListenerV1OrchestrationDependencyException
                actualEventListenerV1OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationDependencyException>(
                        retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualEventListenerV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationDependencyException);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveAllListenerEventV1sIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
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
                service.RetrieveAllListenerEventV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.eventListenerV1OrchestrationService.RetrieveAllListenerEventV1sAsync();

            EventListenerV1OrchestrationServiceException
                actualEventListenerV1OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationServiceException>(
                        retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualEventListenerV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1OrchestrationServiceException);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
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
