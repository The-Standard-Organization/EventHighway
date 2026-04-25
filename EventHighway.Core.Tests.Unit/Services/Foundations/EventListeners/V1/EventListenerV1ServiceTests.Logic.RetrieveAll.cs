// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventListenerV1sAsync()
        {
            // given
            IQueryable<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s();

            IQueryable<EventListenerV1> retrievedEventListenerV1s =
                randomEventListenerV1s;

            IQueryable<EventListenerV1> expectedEventListenerV1s =
                randomEventListenerV1s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenersV1Async())
                    .ReturnsAsync(retrievedEventListenerV1s);

            // when
            IQueryable<EventListenerV1> actualEventListenerV1s =
                await this.eventListenerV1Service
                    .RetrieveAllEventListenersAsync();

            // then
            actualEventListenerV1s.Should().BeEquivalentTo(
                expectedEventListenerV1s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenersV1Async(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
