// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2ProcessingService : IListenerEventArchiveV2ProcessingService
    {
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchiveV2ProcessingService(
            IListenerEventArchiveV2Service listenerEventArchiveV2Service,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync() =>
            TryCatch(async () =>
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync());

        public ValueTask<ListenerEventArchiveV2> AddListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default) => TryCatch(async () =>
        {
            ValidateListenerEventArchiveV2(listenerEventArchiveV2);

            return await this.listenerEventArchiveV2Service
                .AddListenerEventArchiveV2Async(listenerEventArchiveV2, cancellationToken);
        });

        public ValueTask<List<ListenerEventArchiveV2>> RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken) =>
        TryCatch(async () =>
        {
            BatchConfiguration batchConfiguration = this.configurationBroker.GetBatchConfiguration();
            ValidateOnRetrieveNextPurgeBatchOfArchivedEventV2s(olderThan, batchConfiguration);

            IQueryable<ListenerEventArchiveV2> listenerEventArchiveV2 =
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync();

            listenerEventArchiveV2 = FilterListenerEventArchiveV2sOlderThan(
                olderThan, listenerEventArchiveV2)
                    .Take(batchConfiguration.BatchSizeForBulkProcessing);

            return listenerEventArchiveV2.ToList();
        });

        public ValueTask BulkRemoveListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.listenerEventArchiveV2Service.BulkRemoveListenerEventArchiveV2sAsync(
                listenerEventArchiveV2s, cancellationToken);
        });

        private static IQueryable<ListenerEventArchiveV2> FilterListenerEventArchiveV2sOlderThan(
            DateTimeOffset olderThan,
            IQueryable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            listenerEventArchiveV2s = listenerEventArchiveV2s.Where(
                listenerEventArchiveV2 => listenerEventArchiveV2.ArchivedDate < olderThan);

            return listenerEventArchiveV2s;
        }
    }
}
