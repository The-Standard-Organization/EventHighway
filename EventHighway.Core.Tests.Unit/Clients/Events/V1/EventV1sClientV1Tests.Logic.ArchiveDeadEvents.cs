// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Moq;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
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
