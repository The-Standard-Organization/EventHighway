// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        [MemberData(nameof(EventListenerV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnFireScheduledPendingIfDependencyValidationAndLogItAsync(
            Xeption validationException)
        {
            // given
            var expectedEventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventsAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask fireScheduledPendingEventV1sTask =
                this.eventV1CoordinationService.FireScheduledPendingEventsAsync();

            EventV1CoordinationDependencyValidationException
                actualEventV1CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyValidationException>(
                        fireScheduledPendingEventV1sTask.AsTask);

            // then
            expectedEventV1CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyValidationException);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyValidationException))),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenersByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RunEventCallAsync(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        [MemberData(nameof(EventListenerV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnFireScheduledPendingIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedEventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventsAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask fireScheduledPendingEventV1sTask =
                this.eventV1CoordinationService.FireScheduledPendingEventsAsync();

            EventV1CoordinationDependencyException actualEventV1CoordinationDependencyException =
                await Assert.ThrowsAsync<EventV1CoordinationDependencyException>(
                    fireScheduledPendingEventV1sTask.AsTask);

            // then
            actualEventV1CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyException);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyException))),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenersByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RunEventCallAsync(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnFireScheduledPendingIfExceptionOccursAndLogItAsync()
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

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask fireScheduledPendingEventV1sTask =
                this.eventV1CoordinationService.FireScheduledPendingEventsAsync();

            EventV1CoordinationServiceException actualEventV1CoordinationServiceException =
                await Assert.ThrowsAsync<EventV1CoordinationServiceException>(
                    fireScheduledPendingEventV1sTask.AsTask);

            // then
            actualEventV1CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationServiceException);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationServiceException))),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenersByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RunEventCallAsync(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
