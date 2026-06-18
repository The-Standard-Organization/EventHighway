// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventListenerArchiveV2sAsync()
        {
            // given
            IQueryable<EventListenerArchiveV2> randomEventListenerArchiveV2s =
                CreateRandomEventListenerArchiveV2s();

            IQueryable<EventListenerArchiveV2> retrievedEventListenerArchiveV2s =
                randomEventListenerArchiveV2s;

            IQueryable<EventListenerArchiveV2> expectedEventListenerArchiveV2s =
                randomEventListenerArchiveV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerArchiveV2sAsync())
                    .ReturnsAsync(retrievedEventListenerArchiveV2s);

            // when
            IQueryable<EventListenerArchiveV2> actualEventListenerArchiveV2s =
                await this.eventListenerArchiveV2Service
                    .RetrieveAllEventListenerArchiveV2sAsync();

            // then
            actualEventListenerArchiveV2s.Should().BeEquivalentTo(
                expectedEventListenerArchiveV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerArchiveV2sAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
