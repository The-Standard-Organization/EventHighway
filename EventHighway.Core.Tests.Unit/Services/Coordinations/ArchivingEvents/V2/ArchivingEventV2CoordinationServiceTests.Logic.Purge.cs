// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldPurgeArchivedEventV2sAsync()
        {
            // given
            DateTimeOffset olderThan = GetRandomDateTimeOffset();
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.PurgeArchivedEventV2sAsync(
                    olderThan,
                    cancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2CoordinationService
                .PurgeArchivedEventV2sAsync(olderThan, cancellationToken);

            // then
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.PurgeArchivedEventV2sAsync(
                    olderThan,
                    cancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
