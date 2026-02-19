// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientV1Tests
    {
        [Fact]
        public async Task ShouldArchiveDeadEventV1sAsync()
        {
            // given . when
            await this.eventV1SClientV1
                .ArchiveDeadEventV1sAsync();

            // then
            this.eventV1CoordinationServiceV1Mock.Verify(service =>
                service.ArchiveDeadEventV1sAsync(),
                    Times.Once);

            this.eventV1CoordinationServiceV1Mock.VerifyNoOtherCalls();
        }
    }
}
