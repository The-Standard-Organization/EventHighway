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
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Purging;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService : IArchivingEventV2CoordinationService
    {
        private readonly IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService;
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;
        private readonly IListenerEventV2OrchestrationService listenerEventV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2CoordinationService(
            IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService,
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IListenerEventV2OrchestrationService listenerEventV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.archivingEventV2OrchestrationService = archivingEventV2OrchestrationService;
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.listenerEventV2OrchestrationService = listenerEventV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveEventV2sAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exceptions = new List<Exception>();

            try { await ArchiveQuarantinedEventV2sAsync(cancellationToken); }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { exceptions.Add(ex); }

            try { await ArchiveDeadEventsV2sAsync(cancellationToken); }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { exceptions.Add(ex); }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    message: "Failed archiving event service error occurred, contact support.",
                    innerExceptions: exceptions);
            }
        });

        public ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnPurgeEventArchiveV2s(olderThan);
            BatchConfiguration batchConfiguration = this.configurationBroker.GetBatchConfiguration();
            int take = batchConfiguration.BatchSizeForBulkProcessing;
            IEnumerable<EventArchiveV2> batch;

            do
            {
                batch = await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(olderThan, take, cancellationToken);

                if (!batch.Any())
                    break;

                await this.eventArchiveV2OrchestrationService
                    .BulkRemoveEventArchiveV2sAsync(batch, cancellationToken);
            }
            while (true);
        });

        public ValueTask PurgeEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            PurgeConfiguration purgeConfiguration = this.configurationBroker.GetPurgeConfiguration();
            DateTimeOffset currentDateTimeOffset = await this.dateTimeBroker.GetDateTimeOffsetAsync();
            DateTimeOffset olderThan = currentDateTimeOffset.AddDays(-purgeConfiguration.RetentionDays);
            BatchConfiguration batchConfiguration = this.configurationBroker.GetBatchConfiguration();
            int take = batchConfiguration.BatchSizeForBulkProcessing;
            IEnumerable<EventArchiveV2> batch;

            do
            {
                batch = await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(olderThan, take, cancellationToken);

                if (!batch.Any())
                    break;

                await this.eventArchiveV2OrchestrationService
                    .BulkRemoveEventArchiveV2sAsync(batch, cancellationToken);
            }
            while (true);
        });

        private async ValueTask ArchiveQuarantinedEventV2sAsync(CancellationToken cancellationToken)
        {
            var failedEventV2Ids = new List<Guid>();
            IEnumerable<EventV2> quarantinedEventV2s;

            do
            {
                quarantinedEventV2s =
                    await this.archivingEventV2OrchestrationService
                        .RetrieveBatchOfQuarantinedEventV2sAsync(cancellationToken);

                if (!quarantinedEventV2s.Any())
                    break;

                IEnumerable<EventArchiveV2> eventArchiveV2s =
                    quarantinedEventV2s.Select(MapToEventArchiveV2).ToList();

                IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                    await this.eventArchiveV2OrchestrationService
                        .BulkAddEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);

                var archivedIds =
                    addedEventArchiveV2s.Select(archive => archive.Id).ToHashSet();

                IEnumerable<EventV2> removableEventV2s =
                    quarantinedEventV2s
                        .Where(eventV2 => archivedIds.Contains(eventV2.Id)).ToList();

                if (removableEventV2s.Any())
                {
                    await this.archivingEventV2OrchestrationService
                        .BulkRemoveEventV2sAsync(removableEventV2s, cancellationToken);
                }

                foreach (EventV2 unarchivedEventV2 in quarantinedEventV2s
                    .Where(eventV2 => !archivedIds.Contains(eventV2.Id)))
                {
                    failedEventV2Ids.Add(unarchivedEventV2.Id);
                }
            }
            while (true);

            if (failedEventV2Ids.Any())
                await LogFailedArchivingEventV2sAsync(failedEventV2Ids, Array.Empty<Guid>());
        }

        private async ValueTask ArchiveDeadEventsV2sAsync(CancellationToken cancellationToken)
        {
            BatchConfiguration batchConfiguration = this.configurationBroker.GetBatchConfiguration();
            int take = batchConfiguration.BatchSizeForBulkProcessing;

            var faultedEventV2Ids = new HashSet<Guid>();
            var failedEventV2Ids = new List<Guid>();
            var failedListenerEventV2Ids = new List<Guid>();

            IEnumerable<EventV2> deadEventV2s;

            do
            {
                deadEventV2s = await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfDeadEventV2sAsync(cancellationToken);

                IEnumerable<EventV2> pendingDeadEventV2s =
                    deadEventV2s.Where(eventV2 =>
                        !faultedEventV2Ids.Contains(eventV2.Id)).ToList();

                if (!pendingDeadEventV2s.Any())
                    break;

                IEnumerable<EventArchiveV2> eventArchiveV2s =
                    pendingDeadEventV2s.Select(MapToEventArchiveV2).ToList();

                IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                    await this.eventArchiveV2OrchestrationService
                        .BulkAddEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);

                var archivedEventV2Ids =
                    addedEventArchiveV2s.Select(eventArchiveV2 => eventArchiveV2.Id).ToHashSet();

                foreach (EventV2 unarchivedEventV2 in pendingDeadEventV2s
                    .Where(eventV2 => !archivedEventV2Ids.Contains(eventV2.Id)))
                {
                    if (faultedEventV2Ids.Add(unarchivedEventV2.Id))
                        failedEventV2Ids.Add(unarchivedEventV2.Id);
                }

                IEnumerable<Guid> pendingEventV2Ids = archivedEventV2Ids.ToList();

                IEnumerable<ListenerEventV2> listenerEventV2s;

                do
                {
                    listenerEventV2s = await this.listenerEventV2OrchestrationService
                        .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                            pendingEventV2Ids, take, cancellationToken);

                    if (!listenerEventV2s.Any())
                        break;

                    IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s =
                        listenerEventV2s.Select(MapToListenerEventArchiveV2).ToList();

                    IEnumerable<EventArchiveV2> eventArchiveV2sWithListenerEventArchiveV2s =
                        listenerEventArchiveV2s
                            .GroupBy(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                            .Select(listenerEventArchiveV2Group => new EventArchiveV2
                            {
                                Id = listenerEventArchiveV2Group.Key,
                                ListenerEventArchiveV2s = listenerEventArchiveV2Group.ToList()
                            }).ToList();

                    IEnumerable<EventArchiveV2> archivedEventArchiveV2s =
                        await this.eventArchiveV2OrchestrationService
                            .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                                eventArchiveV2sWithListenerEventArchiveV2s, cancellationToken);

                    var addedListenerEventArchiveIds =
                        archivedEventArchiveV2s
                            .SelectMany(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                            .Select(listenerEventArchiveV2 => listenerEventArchiveV2.Id)
                            .ToHashSet();

                    IEnumerable<ListenerEventV2> addedListenerEventV2s =
                        listenerEventV2s
                            .Where(listenerEventV2 =>
                                addedListenerEventArchiveIds.Contains(listenerEventV2.Id)).ToList();

                    if (addedListenerEventV2s.Any())
                    {
                        await this.listenerEventV2OrchestrationService
                            .BulkRemoveListenerEventV2sAsync(addedListenerEventV2s, cancellationToken);
                    }

                    foreach (ListenerEventV2 unarchivedListenerEventV2 in listenerEventV2s
                        .Where(listenerEventV2 =>
                            !addedListenerEventArchiveIds.Contains(listenerEventV2.Id)))
                    {
                        failedListenerEventV2Ids.Add(unarchivedListenerEventV2.Id);
                        faultedEventV2Ids.Add(unarchivedListenerEventV2.EventV2Id);
                    }

                    pendingEventV2Ids =
                        pendingEventV2Ids.Where(eventV2Id =>
                            !faultedEventV2Ids.Contains(eventV2Id)).ToList();

                    if (!pendingEventV2Ids.Any())
                        break;
                }
                while (true);

                IEnumerable<EventV2> removableEventV2s =
                    pendingDeadEventV2s.Where(eventV2 =>
                        archivedEventV2Ids.Contains(eventV2.Id)
                            && !faultedEventV2Ids.Contains(eventV2.Id)).ToList();

                if (removableEventV2s.Any())
                {
                    await this.archivingEventV2OrchestrationService
                        .BulkRemoveEventV2sAsync(removableEventV2s, cancellationToken);
                }
            }
            while (true);

            if (failedEventV2Ids.Any() || failedListenerEventV2Ids.Any())
            {
                await LogFailedArchivingEventV2sAsync(failedEventV2Ids, failedListenerEventV2Ids);
            }
        }

        private async ValueTask LogFailedArchivingEventV2sAsync(
            IEnumerable<Guid> failedEventV2Ids,
            IEnumerable<Guid> failedListenerEventV2Ids)
        {
            var failedArchivingEventV2CoordinationException =
                new FailedArchivingEventV2CoordinationException(
                    message: "Some dead events could not be fully archived " +
                        "and were retained for the next run.");

            if (failedEventV2Ids.Any())
            {
                failedArchivingEventV2CoordinationException.AddData(
                    key: "failedEventV2Ids",
                    values: failedEventV2Ids.Select(id => id.ToString()).ToArray());
            }

            if (failedListenerEventV2Ids.Any())
            {
                failedArchivingEventV2CoordinationException.AddData(
                    key: "failedListenerEventV2Ids",
                    values: failedListenerEventV2Ids.Select(id => id.ToString()).ToArray());
            }

            await this.loggingBroker.LogErrorAsync(failedArchivingEventV2CoordinationException);
        }

        private static EventArchiveV2 MapToEventArchiveV2(EventV2 eventV2)
        {
            return new EventArchiveV2
            {
                Id = eventV2.Id,
                Content = eventV2.Content,
                EventName = eventV2.EventName,
                ContentHash = eventV2.ContentHash,
                Type = (EventArchiveTypeV2)eventV2.Type,
                Status = (EventArchiveStatusV2)eventV2.Status,
                CreatedDate = eventV2.CreatedDate,
                UpdatedDate = eventV2.CreatedDate,
                ScheduledDate = eventV2.ScheduledDate,
                EventAddressV2Id = eventV2.EventAddressV2Id,
                EventParticipantV2Id = eventV2.EventParticipantV2Id,
                EventParticipantV2Secret = eventV2.EventParticipantV2Secret
            };
        }

        private static ListenerEventArchiveV2 MapToListenerEventArchiveV2(
            ListenerEventV2 listenerEventV2)
        {
            return new ListenerEventArchiveV2
            {
                Id = listenerEventV2.Id,
                Status = (ListenerEventArchiveStatusV2)listenerEventV2.Status,
                Response = listenerEventV2.Response,
                ResponseCode = listenerEventV2.ResponseCode,
                ResponseMessage = listenerEventV2.ResponseMessage,
                CreatedDate = listenerEventV2.CreatedDate,
                UpdatedDate = listenerEventV2.CreatedDate,
                EventV2Id = listenerEventV2.EventV2Id,
                EventAddressV2Id = listenerEventV2.EventAddressV2Id,
                EventListenerV2Id = listenerEventV2.EventListenerV2Id,
                EventArchiveV2Id = listenerEventV2.EventV2Id
            };
        }
    }
}
