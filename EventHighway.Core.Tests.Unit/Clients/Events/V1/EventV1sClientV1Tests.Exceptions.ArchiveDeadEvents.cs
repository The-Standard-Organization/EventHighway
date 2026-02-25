// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientV1Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnArchiveIfDependencyValidationErrorOccursAsync()
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

            this.eventV1CoordinationServiceV1Mock.Setup(service =>
                service.ArchiveDeadEventV1sAsync())
                    .ThrowsAsync(eventV1CoordinationDependencyValidationException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1SClientV1.ArchiveDeadEventV1sAsync();

            EventV1ClientDependencyValidationException actualEventV1ClientDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ClientDependencyValidationException>(
                    archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ClientDependencyValidationException);

            this.eventV1CoordinationServiceV1Mock.Verify(service =>
                service.ArchiveDeadEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceV1Mock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnArchiveIfDependencyErrorOccursAsync()
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

            this.eventV1CoordinationServiceV1Mock.Setup(service =>
                service.ArchiveDeadEventV1sAsync())
                    .ThrowsAsync(eventV1CoordinationDependencyException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1SClientV1.ArchiveDeadEventV1sAsync();

            EventV1ClientDependencyException actualEventV1ClientDependencyException =
                await Assert.ThrowsAsync<EventV1ClientDependencyException>(
                    archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ClientDependencyException);

            this.eventV1CoordinationServiceV1Mock.Verify(service =>
                service.ArchiveDeadEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceV1Mock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnArchiveIfServiceErrorOccursAsync()
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

            this.eventV1CoordinationServiceV1Mock.Setup(service =>
                service.ArchiveDeadEventV1sAsync())
                    .ThrowsAsync(eventV1CoordinationServiceException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1SClientV1.ArchiveDeadEventV1sAsync();

            EventV1ClientServiceException actualEventV1ClientServiceException =
                await Assert.ThrowsAsync<EventV1ClientServiceException>(
                    archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1ClientServiceException.Should()
                .BeEquivalentTo(expectedEventV1ClientServiceException);

            this.eventV1CoordinationServiceV1Mock.Verify(service =>
                service.ArchiveDeadEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceV1Mock.VerifyNoOtherCalls();
        }
    }
}
