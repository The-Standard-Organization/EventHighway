// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V1
{
    public partial class ListenerEventV1ProcessingServiceTests
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
                retrievedListenerEventV1s.DeepClone();

            this.listenerEventV1ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventsAsync())
                    .ReturnsAsync(retrievedListenerEventV1s);

            // when
            IQueryable<ListenerEventV1> actualListenerEventV1s =
                await this.listenerEventV1ProcessingService
                    .RetrieveAllListenerEventsAsync();

            // then
            actualListenerEventV1s.Should().BeEquivalentTo(
                expectedListenerEventV1s);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventsAsync(),
                    Times.Once);

            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
