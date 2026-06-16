// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPurgeArchivedEventV2sAsync()
        {
            // given
            DateTimeOffset olderThan = GetRandomDateTimeOffset();
            int batchSize = GetRandomNumber();

            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = batchSize
            };

            List<EventArchiveV2> firstBatch =
                CreateRandomEventArchiveV2sOlderThan(
                    olderThan, count: batchSize);

            List<EventArchiveV2> secondBatch =
                CreateRandomEventArchiveV2sOlderThan(
                    olderThan, count: batchSize - 1);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2ServiceMock.SetupSequence(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync())
                    .ReturnsAsync(firstBatch.AsQueryable())
                    .ReturnsAsync(secondBatch.AsQueryable());

            // when
            await this.eventArchiveV2OrchestrationService
                .PurgeArchivedEventV2sAsync(olderThan, CancellationToken.None);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(),
                    Times.Exactly(2));

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(batch =>
                        batch.SequenceEqual(firstBatch)),
                            It.IsAny<CancellationToken>()),
                            Times.Once);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(batch =>
                        batch.SequenceEqual(secondBatch)),
                            It.IsAny<CancellationToken>()),
                                Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
