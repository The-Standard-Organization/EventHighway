// ---------------------------------------------------------------
// Copyright (c) Aspen Publishing. All rights reserved.
// ---------------------------------------------------------------

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
        public async Task ShouldRetrieveAllEventV1ArchivesAsync()
        {
            // given
            IQueryable<EventV1Archive> randomEventV1Archives =
                CreateRandomEventV1Archives();

            IQueryable<EventV1Archive> serviceEventV1Archives =
                randomEventV1Archives;

            IQueryable<EventV1Archive> expectedEventV1Archives =
                serviceEventV1Archives;

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RetrieveAllEventV1ArchivesAsync())
                    .ReturnsAsync(serviceEventV1Archives);

            // when
            IQueryable<EventV1Archive> actualEventV1Archives =
                await this.eventV1ArchiveProcessingService
                    .RetrieveAllEventV1ArchivesAsync();

            // then
            actualEventV1Archives.Should()
                .BeEquivalentTo(expectedEventV1Archives);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RetrieveAllEventV1ArchivesAsync(),
                    Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
