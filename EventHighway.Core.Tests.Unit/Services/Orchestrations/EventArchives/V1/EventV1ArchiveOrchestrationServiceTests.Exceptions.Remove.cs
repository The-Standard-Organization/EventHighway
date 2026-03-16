// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset inputDateTimeOffset = randomDateTimeOffset;

            ArchiveDeletionPolicy someArchiveDeletionType =
                GetValidEnum<ArchiveDeletionPolicy>();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(inputDateTimeOffset);

            var expectedEventV1ArchiveOrchestrationDependencyValidationException =
                new EventV1ArchiveOrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RemoveEventV1ArchivesAsync(It.IsAny<DateTimeOffset>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask addEventV1ArchiveTask =
                this.eventV1ArchiveOrchestrationService.RemoveEventV1ArchivesAsync(someArchiveDeletionType);

            EventV1ArchiveOrchestrationDependencyValidationException
                actualEventV1ArchiveOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveOrchestrationDependencyValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveOrchestrationDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveOrchestrationDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesAsync(It.IsAny<DateTimeOffset>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveOrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
