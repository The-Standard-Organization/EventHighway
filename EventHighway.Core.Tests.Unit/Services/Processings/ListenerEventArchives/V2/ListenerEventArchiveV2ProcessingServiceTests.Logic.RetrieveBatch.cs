// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveNextPurgeBatchOfArchivedEventV2sAsync()
        {
            // given
            DateTimeOffset olderThan = GetRandomDateTimeOffset();

            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = GetRandomNumber()
            };

            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            List<ListenerEventArchiveV2> archives =
                randomListenerEventArchiveV2s.ToList();

            for (int index = 0; index < archives.Count; index++)
            {
                archives[index].ArchivedDate =
                    index % 2 == 0
                        ? olderThan.AddDays(-1)
                        : olderThan.AddDays(1);
            }

            IQueryable<ListenerEventArchiveV2> retrievedListenerEventArchiveV2s =
                archives.AsQueryable();

            IQueryable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                retrievedListenerEventArchiveV2s
                    .Where(item => item.ArchivedDate < olderThan)
                    .Take(batchConfiguration.BatchSizeForBulkProcessing);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync())
                    .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            List<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        olderThan,
                        CancellationToken.None);

            // then
            actualListenerEventArchiveV2s.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
