// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventHandlerV2sFromStorageAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventHandlerV2> randomEventHandlerV2s = CreateRandomEventHandlerV2s();
            IQueryable<EventHandlerV2> storageEventHandlerV2s = randomEventHandlerV2s;
            IQueryable<EventHandlerV2> expectedEventHandlerV2s = storageEventHandlerV2s;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storageEventHandlerV2s);

            // when
            IQueryable<EventHandlerV2> actualEventHandlerV2s =
                await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    randomCancellationToken);

            // then
            actualEventHandlerV2s.Should().BeEquivalentTo(expectedEventHandlerV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
