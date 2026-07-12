// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Moq;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        [Fact]
        public async Task ShouldFireScheduledPendingEventV1sAsync()
        {
            // given . when
            await this.eventV1SClient
                .FireScheduledPendingEventV1sAsync();

            // then
            this.eventV1CoordinationServiceMock.Verify(service =>
                service.FireScheduledPendingEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
