// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Purging;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldPurgeEventArchiveV2sFromConfigurationAsync()
        {
            // given
            var mockSequence = new MockSequence();

            int randomBatchSize = GetRandomNumber();

            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = randomBatchSize
            };

            int randomRetentionDays = GetRandomNumber();

            var purgeConfiguration = new PurgeConfiguration
            {
                RetentionDays = randomRetentionDays
            };

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset currentDateTimeOffset = randomDateTimeOffset;

            DateTimeOffset expectedOlderThan =
                currentDateTimeOffset.AddDays(-randomRetentionDays);

            IEnumerable<EventArchiveV2> randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> retrievedEventArchiveV2s = randomEventArchiveV2s;
            IEnumerable<EventArchiveV2> emptyEventArchiveV2s = Enumerable.Empty<EventArchiveV2>();

            this.configurationBrokerMock.InSequence(mockSequence)
                .Setup(broker => broker.GetPurgeConfiguration())
                    .Returns(purgeConfiguration);

            this.dateTimeBrokerMock.InSequence(mockSequence)
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(currentDateTimeOffset);

            this.configurationBrokerMock.InSequence(mockSequence)
                .Setup(broker => broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.InSequence(mockSequence)
                .Setup(service =>
                    service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        expectedOlderThan,
                        randomBatchSize,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.InSequence(mockSequence)
                .Setup(service =>
                    service.BulkRemoveEventArchiveV2sAsync(
                        retrievedEventArchiveV2s,
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            this.eventArchiveV2OrchestrationServiceMock.InSequence(mockSequence)
                .Setup(service =>
                    service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        expectedOlderThan,
                        randomBatchSize,
                        randomCancellationToken))
                            .ReturnsAsync(emptyEventArchiveV2s);

            // when
            await this.archivingEventV2CoordinationService
                .PurgeEventArchiveV2sAsync(randomCancellationToken);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetPurgeConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    expectedOlderThan,
                    randomBatchSize,
                    randomCancellationToken),
                        Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    retrievedEventArchiveV2s,
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
