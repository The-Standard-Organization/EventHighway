// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V1
{
    public partial class EventAddressesV1ClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV1ClientDependencyException =
                new EventAddressV1ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",

                    innerException: eventListenerV1OrchestrationDependencyException
                        .InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV1sAsync())
                    .ThrowsAsync(eventListenerV1OrchestrationDependencyException);

            // when
            ValueTask<IQueryable<EventAddressV1>> retrieveAllEventAddressV1sTask =
                this.eventAddressV1sClient.RetrieveAllEventAddressV1sAsync();

            EventAddressV1ClientDependencyException actualEventAddressV1ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV1ClientDependencyException>(
                    retrieveAllEventAddressV1sTask.AsTask);

            // then
            actualEventAddressV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1ClientDependencyException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV1sAsync(),
                    Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAsync()
        {
            // given
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var EventListenerV1OrchestrationServiceException =
                new EventListenerV1OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV1ClientServiceException =
                new EventAddressV1ClientServiceException(
                    message: "Event address client service error occurred, contact support.",

                    innerException: EventListenerV1OrchestrationServiceException
                        .InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV1sAsync())
                    .ThrowsAsync(EventListenerV1OrchestrationServiceException);

            // when
            ValueTask<IQueryable<EventAddressV1>> retrieveAllEventAddressV1sTask =
                this.eventAddressV1sClient.RetrieveAllEventAddressV1sAsync();

            EventAddressV1ClientServiceException actualEventAddressV1ClientServiceException =
                await Assert.ThrowsAsync<EventAddressV1ClientServiceException>(
                    retrieveAllEventAddressV1sTask.AsTask);

            // then
            actualEventAddressV1ClientServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV1ClientServiceException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV1sAsync(),
                    Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
