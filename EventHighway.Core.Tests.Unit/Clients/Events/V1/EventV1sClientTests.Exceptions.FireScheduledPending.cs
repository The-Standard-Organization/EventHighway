// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnFireIfDependencyValidationErrorOccursAsync()
        {
            // given
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    someMessage,
                    someInnerException);

            var expectedEventV1ClientDependencyValidationException =
                new EventV1ClientDependencyValidationException(
                    message: "Event client validation error occurred, fix the errors and try again.",

                    innerException: eventV1CoordinationDependencyValidationException
                        .InnerException as Xeption);

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.FireScheduledPendingEventV1sAsync())
                    .ThrowsAsync(eventV1CoordinationDependencyValidationException);

            // when
            ValueTask fireScheduledPendingEventV1sTask =
                this.eventV1SClient.FireScheduledPendingEventV1sAsync();

            EventV1ClientDependencyValidationException actualEventV1ClientDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ClientDependencyValidationException>(
                    fireScheduledPendingEventV1sTask.AsTask);

            // then
            actualEventV1ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ClientDependencyValidationException);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.FireScheduledPendingEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnFireIfDependencyErrorOccursAsync()
        {
            // given
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventV1ClientDependencyException =
                new EventV1ClientDependencyException(
                    message: "Event client dependency error occurred, contact support.",

                    innerException: eventV1CoordinationDependencyException
                        .InnerException as Xeption);

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.FireScheduledPendingEventV1sAsync())
                    .ThrowsAsync(eventV1CoordinationDependencyException);

            // when
            ValueTask fireScheduledPendingEventV1sTask =
                this.eventV1SClient.FireScheduledPendingEventV1sAsync();

            EventV1ClientDependencyException actualEventV1ClientDependencyException =
                await Assert.ThrowsAsync<EventV1ClientDependencyException>(
                    fireScheduledPendingEventV1sTask.AsTask);

            // then
            actualEventV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ClientDependencyException);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.FireScheduledPendingEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnFireIfServiceErrorOccursAsync()
        {
            // given
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1CoordinationServiceException =
                new EventV1CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventV1ClientServiceException =
                new EventV1ClientServiceException(
                    message: "Event client service error occurred, contact support.",

                    innerException: eventV1CoordinationServiceException
                        .InnerException as Xeption);

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.FireScheduledPendingEventV1sAsync())
                    .ThrowsAsync(eventV1CoordinationServiceException);

            // when
            ValueTask fireScheduledPendingEventV1sTask =
                this.eventV1SClient.FireScheduledPendingEventV1sAsync();

            EventV1ClientServiceException actualEventV1ClientServiceException =
                await Assert.ThrowsAsync<EventV1ClientServiceException>(
                    fireScheduledPendingEventV1sTask.AsTask);

            // then
            actualEventV1ClientServiceException.Should()
                .BeEquivalentTo(expectedEventV1ClientServiceException);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.FireScheduledPendingEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
