// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Moq;

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
                service.FireScheduledPendingEventsAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
