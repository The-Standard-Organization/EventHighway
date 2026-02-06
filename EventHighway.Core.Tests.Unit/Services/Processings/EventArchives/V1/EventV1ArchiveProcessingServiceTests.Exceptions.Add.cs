// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();

            var expectedEventV1ArchiveProcessingDependencyValidationException =
                new EventV1ArchiveProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveProcessingService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveProcessingDependencyValidationException
                actualEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveProcessingDependencyValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyValidationException);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
