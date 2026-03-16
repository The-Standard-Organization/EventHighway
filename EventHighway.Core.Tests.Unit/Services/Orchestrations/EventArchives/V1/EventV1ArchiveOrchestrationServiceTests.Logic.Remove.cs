// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Theory]
        [InlineData(ArchiveDeletionPolicy.Daily)]
        [InlineData(ArchiveDeletionPolicy.Weekly)]
        [InlineData(ArchiveDeletionPolicy.Monthly)]
        [InlineData(ArchiveDeletionPolicy.Quarterly)]
        [InlineData(ArchiveDeletionPolicy.Yearly)]
        [InlineData(ArchiveDeletionPolicy.Duration)]
        public async Task ShouldRemoveEventV1ArchivesAsync(
            ArchiveDeletionPolicy policy)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset inputDateTimeOffset = randomDateTimeOffset;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(inputDateTimeOffset);

            DateTimeOffset expectedCutoffDate =
                (DateTimeOffset)GetCutoffDate(
                    policy, inputDateTimeOffset);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RemoveEventV1ArchivesAsync(
                    expectedCutoffDate))
                        .ReturnsAsync(1);

            // when
            await this.eventV1ArchiveOrchestrationService
                .RemoveEventV1ArchivesAsync(
                    policy);

            // then

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RemoveEventV1ArchivesAsync(
                    expectedCutoffDate),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
