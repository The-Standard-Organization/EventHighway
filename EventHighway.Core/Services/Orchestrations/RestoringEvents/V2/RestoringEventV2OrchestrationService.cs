// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.RestoringEvents.V2
{
    internal partial class RestoringEventV2OrchestrationService : IRestoringEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IEventListenerV2ProcessingService eventListenerV2ProcessingService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public RestoringEventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IEventListenerV2ProcessingService eventListenerV2ProcessingService,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.eventListenerV2ProcessingService = eventListenerV2ProcessingService;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask RestoreAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRestore(eventArchiveV2s, listenerEventArchiveV2s);

            List<EventV2> mappedEventV2s =
                eventArchiveV2s.Select(MapToEventV2).ToList();

            HashSet<System.Guid> incomingEventV2Ids =
                mappedEventV2s.Select(eventV2 => eventV2.Id).ToHashSet();

            IQueryable<EventV2> existingEventV2s =
                await this.eventV2ProcessingService.RetrieveAllEventV2sAsync(cancellationToken);

            HashSet<System.Guid> existingEventV2Ids =
                existingEventV2s
                    .Where(eventV2 => incomingEventV2Ids.Contains(eventV2.Id))
                    .Select(eventV2 => eventV2.Id)
                    .ToHashSet();

            List<EventV2> eventV2sToRestore =
                mappedEventV2s
                    .Where(eventV2 => existingEventV2Ids.Contains(eventV2.Id) is false)
                        .ToList();

            await this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                eventV2sToRestore, cancellationToken);

            List<ListenerEventV2> mappedListenerEventV2s =
                listenerEventArchiveV2s.Select(MapToListenerEventV2).ToList();

            HashSet<System.Guid?> incomingCorrelationIds =
                mappedListenerEventV2s
                    .Select(listenerEventV2 => listenerEventV2.CorrelationId)
                    .ToHashSet();

            IQueryable<ListenerEventV2> existingListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            HashSet<System.Guid?> existingCorrelationIds =
                existingListenerEventV2s
                    .Where(listenerEventV2 =>
                        incomingCorrelationIds.Contains(listenerEventV2.CorrelationId))
                    .Select(listenerEventV2 => listenerEventV2.CorrelationId)
                    .ToHashSet();

            List<ListenerEventV2> listenerEventV2sToRestore =
                mappedListenerEventV2s
                    .Where(listenerEventV2 =>
                        existingCorrelationIds.Contains(listenerEventV2.CorrelationId) is false)
                            .ToList();

            await this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                listenerEventV2sToRestore, cancellationToken);
        });

        public ValueTask GenerateReplayForNewListenersAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnGenerateReplay(eventArchiveV2s);

            HashSet<System.Guid> incomingEventIds =
                eventArchiveV2s.Select(eventArchiveV2 => eventArchiveV2.Id).ToHashSet();

            IQueryable<ListenerEventV2> existingListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            HashSet<(System.Guid EventV2Id, System.Guid EventListenerV2Id)> existingPairs =
                existingListenerEventV2s
                    .Where(listenerEventV2 => incomingEventIds.Contains(listenerEventV2.EventV2Id))
                    .Select(listenerEventV2 => new
                    {
                        listenerEventV2.EventV2Id,
                        listenerEventV2.EventListenerV2Id
                    })
                    .AsEnumerable()
                    .Select(listenerEventV2 =>
                        (listenerEventV2.EventV2Id, listenerEventV2.EventListenerV2Id))
                    .ToHashSet();

            List<ListenerEventV2> generatedListenerEventV2s = new List<ListenerEventV2>();

            foreach (EventArchiveV2 eventArchiveV2 in eventArchiveV2s)
            {
                IQueryable<EventListenerV2> eventListenerV2s =
                    await this.eventListenerV2ProcessingService
                        .RetrieveEventListenerV2sByEventAddressIdAsync(
                            eventArchiveV2.EventAddressV2Id, cancellationToken);

                foreach (EventListenerV2 eventListenerV2 in eventListenerV2s)
                {
                    bool hasListenerEvent =
                        existingPairs.Contains((eventArchiveV2.Id, eventListenerV2.Id));

                    if (hasListenerEvent is false)
                    {
                        generatedListenerEventV2s.Add(
                            GenerateListenerEventV2(eventArchiveV2, eventListenerV2));
                    }
                }
            }

            await this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                generatedListenerEventV2s, cancellationToken);
        });

        public ValueTask GenerateReplayForListenersAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            IEnumerable<System.Guid> eventListenerIds,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnGenerateReplayForListeners(eventArchiveV2s, eventListenerIds);

            HashSet<System.Guid> targetListenerIds = eventListenerIds.ToHashSet();

            HashSet<System.Guid> incomingEventIds =
                eventArchiveV2s.Select(eventArchiveV2 => eventArchiveV2.Id).ToHashSet();

            IQueryable<ListenerEventV2> existingListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            HashSet<(System.Guid EventV2Id, System.Guid EventListenerV2Id)> existingPairs =
                existingListenerEventV2s
                    .Where(listenerEventV2 => incomingEventIds.Contains(listenerEventV2.EventV2Id))
                    .Select(listenerEventV2 => new
                    {
                        listenerEventV2.EventV2Id,
                        listenerEventV2.EventListenerV2Id
                    })
                    .AsEnumerable()
                    .Select(listenerEventV2 =>
                        (listenerEventV2.EventV2Id, listenerEventV2.EventListenerV2Id))
                    .ToHashSet();

            List<ListenerEventV2> generatedListenerEventV2s = new List<ListenerEventV2>();

            foreach (EventArchiveV2 eventArchiveV2 in eventArchiveV2s)
            {
                IQueryable<EventListenerV2> eventListenerV2s =
                    await this.eventListenerV2ProcessingService
                        .RetrieveEventListenerV2sByEventAddressIdAsync(
                            eventArchiveV2.EventAddressV2Id, cancellationToken);

                foreach (EventListenerV2 eventListenerV2 in eventListenerV2s)
                {
                    if (targetListenerIds.Contains(eventListenerV2.Id) is false)
                        continue;

                    bool hasListenerEvent =
                        existingPairs.Contains((eventArchiveV2.Id, eventListenerV2.Id));

                    if (hasListenerEvent is false)
                    {
                        generatedListenerEventV2s.Add(
                            GenerateListenerEventV2(eventArchiveV2, eventListenerV2));
                    }
                }
            }

            await this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                generatedListenerEventV2s, cancellationToken);
        });

        private ListenerEventV2 GenerateListenerEventV2(
            EventArchiveV2 eventArchiveV2,
            EventListenerV2 eventListenerV2)
        {
            RetryConfiguration retryConfiguration =
                this.configurationBroker.GetRetryConfiguration();

            return new ListenerEventV2
            {
                Id = System.Guid.NewGuid(),
                Status = ListenerEventStatusV2.Replay,
                Response = null,
                ResponseCode = null,
                ResponseMessage = null,
                CreatedDate = eventArchiveV2.CreatedDate,
                UpdatedDate = eventArchiveV2.CreatedDate,
                RemainingRetryAttempts = retryConfiguration.RetryAttemptsAllowed,
                RetryAttemptsAllowed = retryConfiguration.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = null,
                DispatchedDate = null,
                EventV2Id = eventArchiveV2.Id,
                EventAddressV2Id = eventArchiveV2.EventAddressV2Id,
                EventListenerV2Id = eventListenerV2.Id,
                EventParticipantV2Id = eventListenerV2.EventParticipantV2Id
            };
        }

        private static EventV2 MapToEventV2(EventArchiveV2 eventArchiveV2) =>
            new EventV2
            {
                Id = eventArchiveV2.Id,
                Content = eventArchiveV2.Content,
                EventName = eventArchiveV2.EventName,
                Type = EventTypeV2.Immediate,
                Status = EventStatusV2.Active,
                CreatedDate = eventArchiveV2.CreatedDate,
                UpdatedDate = eventArchiveV2.UpdatedDate,
                ScheduledDate = null,
                ContentHash = eventArchiveV2.ContentHash,
                EventAddressV2Id = eventArchiveV2.EventAddressV2Id,
                EventParticipantV2Id = eventArchiveV2.EventParticipantV2Id
            };

        private ListenerEventV2 MapToListenerEventV2(
            ListenerEventArchiveV2 listenerEventArchiveV2)
        {
            RetryConfiguration retryConfiguration =
                this.configurationBroker.GetRetryConfiguration();

            return new ListenerEventV2
            {
                Id = System.Guid.NewGuid(),
                CorrelationId = listenerEventArchiveV2.Id,
                Status = ListenerEventStatusV2.Replay,
                Response = null,
                ResponseCode = null,
                ResponseMessage = null,
                CreatedDate = listenerEventArchiveV2.CreatedDate,
                UpdatedDate = listenerEventArchiveV2.UpdatedDate,
                RemainingRetryAttempts = retryConfiguration.RetryAttemptsAllowed,
                RetryAttemptsAllowed = retryConfiguration.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = null,
                DispatchedDate = null,
                EventV2Id = listenerEventArchiveV2.EventV2Id,
                EventAddressV2Id = listenerEventArchiveV2.EventAddressV2Id,
                EventListenerV2Id = listenerEventArchiveV2.EventListenerV2Id,
                EventParticipantV2Id = listenerEventArchiveV2.EventParticipantV2Id
            };
        }
    }
}
