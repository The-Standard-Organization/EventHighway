// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            EventV1Archive nullEventV1Archive = null;

            var nullEventV1ArchiveOrchestrationException =
                new NullEventV1ArchiveOrchestrationException(
                    message: "Event archive is null.");

            var expectedEventV1ArchiveOrchestrationValidationException =
                new EventV1ArchiveOrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ArchiveOrchestrationException);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                    nullEventV1Archive);

            EventV1ArchiveOrchestrationValidationException
                actualEventV1ArchiveOrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveOrchestrationValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveOrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveOrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.eventV1ArchiveServiceMock.Verify(broker =>
                broker.AddEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1ArchivesAreNullAndLogItAsync()
        {
            // given
            var invalidEventV1Archive = new EventV1Archive();
            invalidEventV1Archive.ListenerEventV1Archives = null;

            var nullEventV1ArchiveOrchestrationException =
                new NullListenerEventV1ArchivesOrchestrationException(
                    message: "Listener event archives are null.");

            var expectedEventV1ArchiveOrchestrationValidationException =
                new EventV1ArchiveOrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ArchiveOrchestrationException);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                    invalidEventV1Archive);

            EventV1ArchiveOrchestrationValidationException
                actualEventV1ArchiveOrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveOrchestrationValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveOrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveOrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.eventV1ArchiveServiceMock.Verify(broker =>
                broker.AddEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }
    }
}
