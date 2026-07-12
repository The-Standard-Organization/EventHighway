// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V1
{
    public partial class ListenerEventV1sClientTests
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

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV1sAsync())
                    .ReturnsAsync(retrievedListenerEventV1s);

            // when
            IQueryable<ListenerEventV1> actualListenerEventV1s =
                await this.listenerEventV1sClient
                    .RetrieveAllListenerEventV1sAsync();

            // then
            actualListenerEventV1s.Should().BeEquivalentTo(
                expectedListenerEventV1s);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
