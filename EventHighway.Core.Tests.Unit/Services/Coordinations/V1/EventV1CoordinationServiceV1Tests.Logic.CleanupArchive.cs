// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceV1Tests
    {
        [Theory]
        [InlineData(ArchiveDeletionPolicy.Daily)]
        [InlineData(ArchiveDeletionPolicy.Weekly)]
        [InlineData(ArchiveDeletionPolicy.Monthly)]
        [InlineData(ArchiveDeletionPolicy.Quarterly)]
        [InlineData(ArchiveDeletionPolicy.Yearly)]
        [InlineData(ArchiveDeletionPolicy.Duration)]
        public async Task ShouldCleanupArchiveEventV1sAsync(ArchiveDeletionPolicy policy)
        {
            // given
            this.eventV1ArchiveOrchestrationServiceMock
                .Setup(service => service.RemoveEventV1ArchivesAsync(policy))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.eventV1CoordinationServiceV1
                .CleanUpArchiveDeadEventV1sAsync(policy);

            // then
            this.eventV1ArchiveOrchestrationServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesAsync(policy),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventV1OrchestrationServiceV1Mock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
