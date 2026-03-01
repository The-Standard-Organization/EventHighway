// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV1ArchiveValidationExceptions))]
        [MemberData(nameof(ListenerEventV1ArchiveValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();

            var expectedEventV1ArchiveOrchestrationDependencyValidationException =
                new EventV1ArchiveOrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                    someEventV1Archive);

            EventV1ArchiveOrchestrationDependencyValidationException
                actualEventV1ArchiveOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveOrchestrationDependencyValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveOrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveOrchestrationDependencyValidationException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveOrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.Verify(broker =>
                broker.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Never);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1ArchiveDependencyExceptions))]
        [MemberData(nameof(ListenerEventV1ArchiveDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();

            var expectedEventV1ArchiveOrchestrationDependencyException =
                new EventV1ArchiveOrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                    someEventV1Archive);

            EventV1ArchiveOrchestrationDependencyException
                actualEventV1ArchiveOrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventV1ArchiveOrchestrationDependencyException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveOrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveOrchestrationDependencyException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveOrchestrationDependencyException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.Verify(broker =>
                broker.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Never);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();
            var exception = new Exception();

            var failedEventV1ArchiveOrchestrationServiceException =
                new FailedEventV1ArchiveOrchestrationServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: exception);

            var expectedEventV1ArchiveOrchestrationServiceException =
                new EventV1ArchiveOrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventV1ArchiveOrchestrationServiceException);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()))
                    .ThrowsAsync(exception);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                    someEventV1Archive);

            EventV1ArchiveOrchestrationServiceException
                actualEventV1ArchiveOrchestrationServiceException =
                    await Assert.ThrowsAsync<EventV1ArchiveOrchestrationServiceException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveOrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveOrchestrationServiceException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveOrchestrationServiceException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.Verify(broker =>
                broker.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Never);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }
    }
}
