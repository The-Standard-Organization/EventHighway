// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            EventV1Archive nullEventV1Archive = null;

            var nullEventV1ArchiveProcessingException =
                new NullEventV1ArchiveProcessingException(
                    message: "Event archive is null.");

            var expectedEventV1ArchiveProcessingValidationException =
                new EventV1ArchiveProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ArchiveProcessingException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveProcessingService.AddEventV1ArchiveAsync(nullEventV1Archive);

            EventV1ArchiveProcessingValidationException
                actualEventV1ArchiveProcessingValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveProcessingValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveProcessingValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.AddEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
        }
    }
}
