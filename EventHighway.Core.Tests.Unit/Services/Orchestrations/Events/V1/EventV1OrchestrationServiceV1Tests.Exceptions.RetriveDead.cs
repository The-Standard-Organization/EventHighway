// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceV1Tests
    {
        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDeadEventsIfEventV1DependencyAndLogItAsync(
            Xeption eventV1DependencyException)
        {
            // given
            var expectedEventV1OrchestrationDependencyException =
                new EventV1OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV1DependencyException.InnerException as Xeption);

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV1sWithListenersAsync())
                    .ThrowsAsync(eventV1DependencyException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllDeadEventV1sWithListenersTask =
                this.eventV1OrchestrationServiceV1.RetrieveAllDeadEventV1sWithListenersAsync();

            EventV1OrchestrationDependencyException actualEventV1OrchestrationDependencyException =
                await Assert.ThrowsAsync<EventV1OrchestrationDependencyException>(
                    retrieveAllDeadEventV1sWithListenersTask.AsTask);

            // then
            actualEventV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyException);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV1sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveDeadIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedEventV1OrchestrationServiceException =
                new FailedEventV1OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1OrchestrationServiceException =
                new EventV1OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1OrchestrationServiceException);

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllDeadEventV1sWithListenersTask =
                this.eventV1OrchestrationServiceV1.RetrieveAllDeadEventV1sWithListenersAsync();

            EventV1OrchestrationServiceException actualEventV1OrchestrationServiceException =
                await Assert.ThrowsAsync<EventV1OrchestrationServiceException>(
                    retrieveAllDeadEventV1sWithListenersTask.AsTask);

            // then
            actualEventV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationServiceException);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationServiceException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
