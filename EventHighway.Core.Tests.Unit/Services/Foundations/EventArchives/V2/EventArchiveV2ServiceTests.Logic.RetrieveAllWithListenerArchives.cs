// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync()
        {
            // given
            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IQueryable<EventArchiveV2> retrievedEventArchiveV2s =
                randomEventArchiveV2s;

            IQueryable<EventArchiveV2> expectedEventArchiveV2s =
                retrievedEventArchiveV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync())
                    .ReturnsAsync(retrievedEventArchiveV2s);

            // when
            IQueryable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service
                    .RetrieveAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync();

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(
                expectedEventArchiveV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
