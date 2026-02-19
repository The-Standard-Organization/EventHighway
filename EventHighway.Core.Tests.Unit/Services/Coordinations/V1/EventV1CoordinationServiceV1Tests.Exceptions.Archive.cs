// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
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
        public async Task ShouldThrowDependencyValidationOnArchiveDeadEventV1sIfDependencyValidationErrorAndLogItAsync(
            Xeption validationException)
        {
            // given
            var expectedEventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1OrchestrationServiceV1Mock.Setup(service =>
                service.RetrieveAllDeadEventV1sWithListenersAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask archiveDeadEventV1sTask =
                this.eventV1CoordinationServiceV1.ArchiveDeadEventV1sAsync();

            EventV1CoordinationDependencyValidationException
                actualEventV1CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyValidationException>(
                        archiveDeadEventV1sTask.AsTask);

            // then
            actualEventV1CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyValidationException);

            this.eventV1OrchestrationServiceV1Mock.Verify(service =>
                service.RetrieveAllDeadEventV1sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventV1ArchiveOrchestrationServiceMock.Verify(service =>
                service.AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.eventV1OrchestrationServiceV1Mock.Verify(service =>
                service.RemoveEventV1AndListenerEventV1sAsync(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceV1Mock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
