// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Services.Orchestrations.Events.V1;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal partial class EventV1CoordinationServiceV1 : IEventV1CoordinationServiceV1
    {
        private readonly IEventV1OrchestrationServiceV1 eventV1OrchestrationServiceV1;
        private readonly IEventV1ArchiveOrchestrationService eventV1ArchiveOrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1CoordinationServiceV1(
            IEventV1OrchestrationServiceV1 eventV1OrchestrationServiceV1,
            IEventV1ArchiveOrchestrationService eventV1ArchiveOrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV1OrchestrationServiceV1 = eventV1OrchestrationServiceV1;
            this.eventV1ArchiveOrchestrationService = eventV1ArchiveOrchestrationService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveDeadEventV1sAsync() =>
        TryCatch(async () =>
        {
            IQueryable<EventV1> eventV1s =
                await this.eventV1OrchestrationServiceV1
                    .RetrieveAllDeadEventV1sWithListenersAsync();

            foreach (EventV1 eventV1 in eventV1s)
            {
                EventV1Archive eventV1Archive =
                    await MapToEventV1ArchiveAsync(eventV1);

                await this.eventV1ArchiveOrchestrationService
                    .AddEventV1ArchiveWithListenerEventV1ArchivesAsync(
                        eventV1Archive);

                await this.eventV1OrchestrationServiceV1
                    .RemoveEventV1AndListenerEventV1sAsync(
                        eventV1);
            }
        });

        private async ValueTask<EventV1Archive> MapToEventV1ArchiveAsync(
            EventV1 eventV1)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return new EventV1Archive
            {
                Id = eventV1.Id,
                Content = eventV1.Content,
                Type = (EventV1ArchiveType)eventV1.Type,
                CreatedDate = eventV1.CreatedDate,
                UpdatedDate = eventV1.UpdatedDate,
                ScheduledDate = eventV1.ScheduledDate,
                ArchivedDate = currentDateTime,
                EventAddressId = eventV1.EventAddressId,

                ListenerEventV1Archives = eventV1.ListenerEvents
                    ?.Select(listenerEvent =>
                        MapToListenerEventV1Archive(
                            listenerEvent,
                            currentDateTime))
                                .ToList()
            };
        }

        private ListenerEventV1Archive MapToListenerEventV1Archive(
            ListenerEventV1 listenerEventV1,
            DateTimeOffset currentDateTime)
        {
            return new ListenerEventV1Archive
            {
                Id = listenerEventV1.Id,
                Status = (ListenerEventV1ArchiveStatus)listenerEventV1.Status,
                Response = listenerEventV1.Response,
                CreatedDate = listenerEventV1.CreatedDate,
                UpdatedDate = listenerEventV1.UpdatedDate,
                ArchivedDate = currentDateTime,
                EventId = listenerEventV1.EventId,
                EventAddressId = listenerEventV1.EventAddressId,
                EventListenerId = listenerEventV1.EventListenerId
            };
        }
    }
}
