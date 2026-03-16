// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceV1Tests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        [MemberData(nameof(EventV1ArchiveValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnCleanUpArchiveDeadEventV1sIfDependencyValidationErrorAndLogItAsync(
            Xeption validationException)
        {
            // given
            var expectedEventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ArchiveOrchestrationServiceMock.Setup(service =>
                service.RemoveEventV1ArchivesByDeletionPolicyAsync(ArchiveDeletionPolicy.Monthly))
                    .ThrowsAsync(validationException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1CoordinationServiceV1.CleanUpArchiveDeadEventV1sAsync(ArchiveDeletionPolicy.Monthly);

            EventV1CoordinationDependencyValidationException
                actualEventV1CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyValidationException>(
                        archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyValidationException);

            this.eventV1ArchiveOrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesByDeletionPolicyAsync(ArchiveDeletionPolicy.Monthly),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventV1ArchiveOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        [MemberData(nameof(EventV1ArchiveDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnCleanUpArchiveDeadEventV1sIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedEventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1ArchiveOrchestrationServiceMock.Setup(service =>
                service.RemoveEventV1ArchivesByDeletionPolicyAsync(ArchiveDeletionPolicy.Monthly))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1CoordinationServiceV1.CleanUpArchiveDeadEventV1sAsync(
                    ArchiveDeletionPolicy.Monthly);

            EventV1CoordinationDependencyException
                actualEventV1CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyException>(
                        archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyException);

            this.eventV1ArchiveOrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesByDeletionPolicyAsync(ArchiveDeletionPolicy.Monthly),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventV1ArchiveOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnCleanUpArchiveDeadEventV1sIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedEventV1CoordinationServiceException =
                new FailedEventV1CoordinationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1CoordinationServiceException =
                new EventV1CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1CoordinationServiceException);

            this.eventV1ArchiveOrchestrationServiceMock.Setup(service =>
                service.RemoveEventV1ArchivesByDeletionPolicyAsync(ArchiveDeletionPolicy.Monthly))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1CoordinationServiceV1.CleanUpArchiveDeadEventV1sAsync(
                    ArchiveDeletionPolicy.Monthly);

            EventV1CoordinationServiceException
                actualEventV1CoordinationServiceException =
                    await Assert.ThrowsAsync<EventV1CoordinationServiceException>(
                        archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationServiceException);

            this.eventV1ArchiveOrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesByDeletionPolicyAsync(ArchiveDeletionPolicy.Monthly),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventV1ArchiveOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
