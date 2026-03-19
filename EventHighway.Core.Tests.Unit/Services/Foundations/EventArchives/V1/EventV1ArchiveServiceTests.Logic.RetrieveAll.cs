// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventV1ArchivesAsync()
        {
            // given
            IQueryable<EventV1Archive> randomEventV1Archives =
                CreateRandomEventV1Archives();

            IQueryable<EventV1Archive> retrievedEventV1Archives =
                randomEventV1Archives;

            IQueryable<EventV1Archive> expectedEventV1Archives =
                randomEventV1Archives.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventV1ArchivesAsync())
                    .ReturnsAsync(retrievedEventV1Archives);

            // when
            IQueryable<EventV1Archive> actualEventV1Archives =
                await this.eventV1ArchiveService
                    .RetrieveAllEventV1ArchivesAsync();

            // then
            actualEventV1Archives.Should().BeEquivalentTo(
                expectedEventV1Archives);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV1ArchivesAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
