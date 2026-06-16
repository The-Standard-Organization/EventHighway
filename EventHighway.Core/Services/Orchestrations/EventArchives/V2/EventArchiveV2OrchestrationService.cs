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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal partial class EventArchiveV2OrchestrationService : IEventArchiveV2OrchestrationService
    {
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2OrchestrationService(
            IListenerEventArchiveV2Service listenerEventArchiveV2Service,
            IEventArchiveV2Service eventArchiveV2Service,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync());

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync());

        public ValueTask AddEventArchiveV2WithListenerEventArchiveV2sAsync(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2(eventArchiveV2);
            await this.eventArchiveV2Service.AddEventArchiveV2Async(eventArchiveV2, cancellationToken);

            foreach (ListenerEventArchiveV2 listenerEventArchiveV2 in eventArchiveV2.ListenerEventArchiveV2s)
            {
                await this.listenerEventArchiveV2Service
                    .AddListenerEventArchiveV2Async(listenerEventArchiveV2, cancellationToken);
            }
        });

        public ValueTask PurgeArchivedEventV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            BatchConfiguration batchConfiguration = this.configurationBroker.GetBatchConfiguration();
            ValidateOnRetrieveNextPurgeBatchOfArchivedEventV2s(olderThan, batchConfiguration);

            while (true)
            {
                List<EventArchiveV2> eventArchiveV2Batch =
                    await RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        olderThan, batchConfiguration.BatchSizeForBulkProcessing, cancellationToken);

                if (eventArchiveV2Batch.Count == 0)
                    break;

                IEnumerable<Guid> eventArchiveV2Ids = eventArchiveV2Batch.Select(e => e.Id);

                while (true)
                {
                    List<ListenerEventArchiveV2> listenerEventArchiveV2Batch =
                        await RetrieveNextPurgeBatchOfListenerEventArchiveV2sAsync(
                            eventArchiveV2Ids, batchConfiguration.BatchSizeForBulkProcessing, cancellationToken);

                    if (listenerEventArchiveV2Batch.Count == 0)
                        break;

                    await this.listenerEventArchiveV2Service
                        .BulkRemoveListenerEventArchiveV2sAsync(listenerEventArchiveV2Batch, cancellationToken);

                    if (listenerEventArchiveV2Batch.Count < batchConfiguration.BatchSizeForBulkProcessing)
                        break;
                }

                await this.eventArchiveV2Service
                    .BulkRemoveEventArchiveV2sAsync(eventArchiveV2Batch, cancellationToken);

                if (eventArchiveV2Batch.Count < batchConfiguration.BatchSizeForBulkProcessing)
                    break;
            }
        });

        private async ValueTask<List<EventArchiveV2>> RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
            DateTimeOffset olderThan,
            int batchSizeForBulkProcessing,
            CancellationToken cancellationToken)
        {
            IQueryable<EventArchiveV2> eventArchiveV2s =
                  await eventArchiveV2Service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync();

            IQueryable<EventArchiveV2> filteredEventArchiveV2s = FilterEventArchiveV2sOlderThan(
                olderThan, eventArchiveV2s)
                    .Take(batchSizeForBulkProcessing);

            return filteredEventArchiveV2s.ToList();
        }
        private async ValueTask<List<ListenerEventArchiveV2>> RetrieveNextPurgeBatchOfListenerEventArchiveV2sAsync(
            IEnumerable<Guid> eventArchiveV2Ids,
            int batchSizeForBulkProcessing,
            CancellationToken cancellationToken)
        {
            IQueryable<EventArchiveV2> eventArchiveV2s =
                    await eventArchiveV2Service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync();

            IQueryable<ListenerEventArchiveV2> filteredListenerEventArchiveV2s = FilterListenerEventArchiveV2sOlderThan(
                eventArchiveV2Ids, eventArchiveV2s)
                    .Take(batchSizeForBulkProcessing);

            return filteredListenerEventArchiveV2s.ToList();
        }

        private static IQueryable<EventArchiveV2> FilterEventArchiveV2sOlderThan(
            DateTimeOffset olderThan,
            IQueryable<EventArchiveV2> eventArchiveV2s)
        {
            eventArchiveV2s = eventArchiveV2s.Where(
                eventArchiveV2 => eventArchiveV2.ArchivedDate < olderThan);

            return eventArchiveV2s;
        }

        private static IQueryable<ListenerEventArchiveV2> FilterListenerEventArchiveV2sOlderThan(
           IEnumerable<Guid> eventArchiveV2Ids,
           IQueryable<EventArchiveV2> eventArchiveV2s)
        {
            return eventArchiveV2s
                .Where(eventArchiveV2 =>
                    eventArchiveV2Ids.Contains(eventArchiveV2.Id))
                        .SelectMany(eventArchiveV2 =>
                            eventArchiveV2.ListenerEventArchiveV2s);
        }
    }
}
