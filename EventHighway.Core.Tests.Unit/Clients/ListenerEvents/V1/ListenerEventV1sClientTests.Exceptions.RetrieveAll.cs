// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V1
{
    public partial class ListenerEventV1sClientTests
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

            var expectedListenerEventV1ClientDependencyException =
                new ListenerEventV1ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: eventListenerV1OrchestrationDependencyException
                        .InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV1sAsync())
                    .ThrowsAsync(eventListenerV1OrchestrationDependencyException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.listenerEventV1sClient.RetrieveAllListenerEventV1sAsync();

            ListenerEventV1ClientDependencyException actualListenerEventV1ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV1ClientDependencyException>(
                    retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualListenerEventV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ClientDependencyException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
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

            var expectedListenerEventV1ClientServiceException =
                new ListenerEventV1ClientServiceException(
                    message: "Listener event client service error occurred, contact support.",

                    innerException: EventListenerV1OrchestrationServiceException
                        .InnerException as Xeption);

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV1sAsync())
                    .ThrowsAsync(EventListenerV1OrchestrationServiceException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.listenerEventV1sClient.RetrieveAllListenerEventV1sAsync();

            ListenerEventV1ClientServiceException actualListenerEventV1ClientServiceException =
                await Assert.ThrowsAsync<ListenerEventV1ClientServiceException>(
                    retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualListenerEventV1ClientServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ClientServiceException);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
