// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDeadEventsIfEventV1DependencyAndLogItAsync(
            Xeption eventV1DependencyException)
        {
            // given
            var expectedEventV1ProcessingDependencyException =
                new EventV1ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV1DependencyException.InnerException as Xeption);

            this.eventV1ServiceMock.Setup(service =>
                service.RetrieveAllEventV1sWithListenerEventV1sAsync())
                    .ThrowsAsync(eventV1DependencyException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllDeadEventV1sWithListenersTask =
                this.eventV1ProcessingService.RetrieveAllDeadEventV1sWithListenersAsync();

            EventV1ProcessingDependencyException actualEventV1ProcessingDependencyException =
                await Assert.ThrowsAsync<EventV1ProcessingDependencyException>(
                    retrieveAllDeadEventV1sWithListenersTask.AsTask);

            // then
            actualEventV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingDependencyException);

            this.eventV1ServiceMock.Verify(service =>
                service.RetrieveAllEventV1sWithListenerEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingDependencyException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveDeadEventsIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedEventV1ProcessingServiceException =
                new FailedEventV1ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1ProcessingServiceException =
                new EventV1ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1ProcessingServiceException);

            this.eventV1ServiceMock.Setup(service =>
                service.RetrieveAllEventV1sWithListenerEventV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllDeadEventV1sWithListenersTask =
                this.eventV1ProcessingService.RetrieveAllDeadEventV1sWithListenersAsync();

            EventV1ProcessingServiceException actualEventV1ProcessingServiceException =
                await Assert.ThrowsAsync<EventV1ProcessingServiceException>(
                    retrieveAllDeadEventV1sWithListenersTask.AsTask);

            // then
            actualEventV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventV1ProcessingServiceException);

            this.eventV1ServiceMock.Verify(service =>
                service.RetrieveAllEventV1sWithListenerEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ProcessingServiceException))),
                        Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
