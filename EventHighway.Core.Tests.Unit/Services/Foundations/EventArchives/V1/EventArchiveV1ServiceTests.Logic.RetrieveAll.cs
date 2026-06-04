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
    public partial class EventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventArchiveV1sAsync()
        {
            // given
            IQueryable<EventArchiveV1> randomEventArchiveV1s =
                CreateRandomEventArchiveV1s();

            IQueryable<EventArchiveV1> retrievedEventArchiveV1s =
                randomEventArchiveV1s;

            IQueryable<EventArchiveV1> expectedEventArchiveV1s =
                randomEventArchiveV1s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV1sAsync())
                    .ReturnsAsync(retrievedEventArchiveV1s);

            // when
            IQueryable<EventArchiveV1> actualEventArchiveV1s =
                await this.eventArchiveV1Service
                    .RetrieveAllEventArchiveV1sAsync();

            // then
            actualEventArchiveV1s.Should().BeEquivalentTo(
                expectedEventArchiveV1s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV1sAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
