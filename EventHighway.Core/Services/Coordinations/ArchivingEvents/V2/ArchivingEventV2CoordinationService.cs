// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService : IArchivingEventV2CoordinationService
    {
        private readonly IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService;
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2CoordinationService(
            IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService,
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.archivingEventV2OrchestrationService = archivingEventV2OrchestrationService;
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await foreach (EventV2 eventV2 in
                this.archivingEventV2OrchestrationService
                    .RetrieveAllDeadEventV2sWithListenersAsync(cancellationToken))
            {
                EventArchiveV2 eventArchiveV2 =
                    await MapToEventArchiveV2Async(eventV2);

                await this.eventArchiveV2OrchestrationService
                    .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        eventArchiveV2,
                        cancellationToken);

                await this.archivingEventV2OrchestrationService
                    .RemoveEventV2AndListenerEventV2sAsync(
                        eventV2,
                        cancellationToken);
            }
        });

        private async ValueTask<EventArchiveV2> MapToEventArchiveV2Async(EventV2 eventV2)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return new EventArchiveV2
            {
                Id = eventV2.Id,
                Content = eventV2.Content,
                EventName = eventV2.EventName,
                Type = (EventArchiveTypeV2)eventV2.Type,
                CreatedDate = eventV2.CreatedDate,
                UpdatedDate = eventV2.UpdatedDate,
                ScheduledDate = eventV2.ScheduledDate,
                RemainingRetryAttempts = eventV2.RemainingRetryAttempts,
                ArchivedDate = currentDateTime,
                EventAddressId = eventV2.EventAddressId,

                ListenerEventArchiveV2s = eventV2.ListenerEventV2s
                    ?.Select(listenerEventV2 =>
                        MapToListenerEventArchiveV2(
                            listenerEventV2,
                            currentDateTime))
                                .ToList()
            };
        }

        private static ListenerEventArchiveV2 MapToListenerEventArchiveV2(
            ListenerEventV2 listenerEventV2,
            DateTimeOffset currentDateTime)
        {
            return new ListenerEventArchiveV2
            {
                Id = listenerEventV2.Id,
                Status = (ListenerEventArchiveStatusV2)listenerEventV2.Status,
                Response = listenerEventV2.Response,
                ResponseCode = listenerEventV2.ResponseCode,
                ResponseMessage = listenerEventV2.ResponseMessage,
                CreatedDate = listenerEventV2.CreatedDate,
                UpdatedDate = listenerEventV2.UpdatedDate,
                ArchivedDate = currentDateTime,
                EventId = listenerEventV2.EventId,
                EventAddressId = listenerEventV2.EventAddressId,
                EventListenerId = listenerEventV2.EventListenerId,
                EventArchiveV2Id = listenerEventV2.EventId
            };
        }
    }
}
