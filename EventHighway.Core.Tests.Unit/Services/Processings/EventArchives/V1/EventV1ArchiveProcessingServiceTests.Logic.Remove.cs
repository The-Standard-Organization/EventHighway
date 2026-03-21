// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Fact]
        private async Task ShouldRemoveEventV1ArchivesAsync()
        {
            // given
            DateTimeOffset randomCutOffDate = GetRandomDateTimeOffset();
            DateTimeOffset inputCutOffDate = randomCutOffDate;

            IQueryable<EventV1Archive> randomEventV1Archives =
                CreateRandomEventV1Archives();

            randomEventV1Archives.FirstOrDefault().ArchivedDate = 
                inputCutOffDate.AddDays(-GetRandomNumber());

            IQueryable<EventV1Archive> retrievedEventV1Archives =
                randomEventV1Archives;

            IQueryable<EventV1Archive> filteredEventV1Archives =
                retrievedEventV1Archives
                    .Where(eventV1Archive => eventV1Archive.ArchivedDate < inputCutOffDate);

            this.eventV1ArchiveServiceMock
                .Setup(service => service.RetrieveAllEventV1ArchivesAsync())
                    .ReturnsAsync(retrievedEventV1Archives);

            foreach (EventV1Archive eventV1Archive in filteredEventV1Archives)
            {
                this.eventV1ArchiveServiceMock.Setup(service => 
                        service.RemoveEventV1ArchiveByIdAsync(eventV1Archive.Id))
                            .ReturnsAsync(eventV1Archive);
            }

            // when
            await this.eventV1ArchiveProcessingService
                .RemoveEventV1ArchivesAsync(inputCutOffDate);

            // then
            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RetrieveAllEventV1ArchivesAsync(),
                    Times.Once);

            foreach (EventV1Archive eventV1Archive in filteredEventV1Archives)
            {
                this.eventV1ArchiveServiceMock.Verify(service =>
                    service.RemoveEventV1ArchiveByIdAsync(eventV1Archive.Id),
                        Times.Once);
            }

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
