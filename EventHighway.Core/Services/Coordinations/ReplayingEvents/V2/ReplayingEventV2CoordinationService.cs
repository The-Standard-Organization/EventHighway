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
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.RestoringEvents.V2;

namespace EventHighway.Core.Services.Coordinations.ReplayingEvents.V2
{
    internal partial class ReplayingEventV2CoordinationService : IReplayingEventV2CoordinationService
    {
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;
        private readonly IRestoringEventV2OrchestrationService restoringEventV2OrchestrationService;
        private readonly IReplayingListenerEventV2OrchestrationService replayingListenerEventV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReplayingEventV2CoordinationService(
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IRestoringEventV2OrchestrationService restoringEventV2OrchestrationService,
            IReplayingListenerEventV2OrchestrationService replayingListenerEventV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.restoringEventV2OrchestrationService = restoringEventV2OrchestrationService;
            this.replayingListenerEventV2OrchestrationService = replayingListenerEventV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnReplay(startDate, endDate);

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            int take = batchConfiguration.BatchSizeForBulkProcessing;
            int skip = 0;
            IEnumerable<EventArchiveV2> eventArchiveV2Batch;

            do
            {
                eventArchiveV2Batch = await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sMatchingAsync(
                        eventAddressId, eventListenerIds, startDate, endDate, skip, take, cancellationToken);

                if (!eventArchiveV2Batch.Any())
                    break;

                foreach (EventArchiveV2 eventArchiveV2 in eventArchiveV2Batch)
                {
                    int listenerArchiveSkip = 0;
                    IEnumerable<ListenerEventArchiveV2> listenerArchivePage;

                    do
                    {
                        EventArchiveV2 eventArchiveV2WithListenerArchivePage =
                            await this.eventArchiveV2OrchestrationService
                                .RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                                    eventArchiveV2.Id,
                                    eventListenerIds,
                                    startDate,
                                    endDate,
                                    listenerArchiveSkip,
                                    take,
                                    cancellationToken);

                        listenerArchivePage =
                            eventArchiveV2WithListenerArchivePage.ListenerEventArchiveV2s;

                        if (listenerArchivePage is null || !listenerArchivePage.Any())
                            break;

                        await this.restoringEventV2OrchestrationService
                            .RestoreAsync(
                                new[] { eventArchiveV2 },
                                listenerArchivePage,
                                cancellationToken);

                        if (take == 0)
                            break;

                        listenerArchiveSkip += take;
                    }
                    while (true);
                }

                if (eventListenerIds is null || !eventListenerIds.Any())
                {
                    await this.restoringEventV2OrchestrationService
                        .GenerateReplayForNewListenersAsync(eventArchiveV2Batch, cancellationToken);
                }

                if (take == 0)
                    break;

                skip += take;
            }
            while (true);
        });

        public ValueTask ReplayEventArchiveV2sAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnTargetedReplay(eventV2Id, eventListenerIds);

            IEnumerable<EventArchiveV2> eventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2sByIdsAsync(
                        new[] { eventV2Id }, cancellationToken);

            EventArchiveV2 eventArchiveV2 = eventArchiveV2s.FirstOrDefault();

            if (eventArchiveV2 is null)
                return;

            if (eventAddressId.HasValue
                && eventArchiveV2.EventAddressV2Id != eventAddressId.Value)
            {
                return;
            }

            if (eventArchiveV2.Status is EventArchiveStatusV2.Quarantined
                && allowReplayOfQuarantinedItem is false)
            {
                return;
            }

            var targetedEventArchiveV2s =
                new List<EventArchiveV2> { eventArchiveV2 };

            await this.restoringEventV2OrchestrationService
                .RestoreAsync(
                    targetedEventArchiveV2s,
                    new List<ListenerEventArchiveV2>(),
                    cancellationToken);

            await this.restoringEventV2OrchestrationService
                .GenerateReplayForListenersAsync(
                    targetedEventArchiveV2s, eventListenerIds, cancellationToken);
        });

        public ValueTask ProcessReplayedListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            int take = batchConfiguration.BatchSizeForBulkProcessing;
            IEnumerable<ListenerEventV2> listenerEventV2Batch;

            do
            {
                listenerEventV2Batch =
                    await this.replayingListenerEventV2OrchestrationService
                        .RetrieveBatchOfReplayListenerEventV2sAsync(take, cancellationToken);

                if (!listenerEventV2Batch.Any())
                    break;

                foreach (ListenerEventV2 listenerEventV2 in listenerEventV2Batch)
                {
                    try
                    {
                        await this.replayingListenerEventV2OrchestrationService
                            .ProcessReplayListenerEventV2Async(listenerEventV2, cancellationToken);
                    }
                    catch (Exception exception)
                        when (exception is not OperationCanceledException)
                    {
                        await this.loggingBroker.LogErrorAsync(exception);
                    }
                }

                if (take == 0)
                    break;
            }
            while (true);
        });
    }
}
