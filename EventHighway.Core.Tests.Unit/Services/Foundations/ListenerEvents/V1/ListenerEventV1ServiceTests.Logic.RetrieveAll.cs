// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventV1sAsync()
        {
            // given
            IQueryable<ListenerEventV1> randomListenerEventV1s =
                CreateRandomListenerEventV1s();

            IQueryable<ListenerEventV1> retrievedListenerEventV1s =
                randomListenerEventV1s;

            IQueryable<ListenerEventV1> expectedListenerEventV1s =
                randomListenerEventV1s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventsV1Async())
                    .ReturnsAsync(retrievedListenerEventV1s);

            // when
            IQueryable<ListenerEventV1> actualListenerEventV1s =
                await this.listenerEventV1Service
                    .RetrieveAllListenerEventsAsync();

            // then
            actualListenerEventV1s.Should().BeEquivalentTo(
                expectedListenerEventV1s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventsV1Async(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
