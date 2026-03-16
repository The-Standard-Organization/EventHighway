// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfEventV1ArchiveDeletionTypeIsInvalidAndLogItAsync()
        {
            // given
            ArchiveDeletionPolicy invalidArchiveDeletionType =
                GetInvalidEnum<ArchiveDeletionPolicy>();

            var invalidEventV1ArchiveException =
                new InvalidEventV1ArchiveOrchestrationException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventV1Archive),
                values: "Value is not recognized");

            var expectedEventV1ArchiveOrchestrationValidationException =
                new EventV1ArchiveOrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.RemoveEventV1ArchivesAsync(invalidArchiveDeletionType);

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

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesAsync(
                    It.IsAny<DateTimeOffset>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }
    }
}
