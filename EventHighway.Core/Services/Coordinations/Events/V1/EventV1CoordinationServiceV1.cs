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
        private readonly IEventArchiveV1OrchestrationService eventArchiveV1OrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1CoordinationServiceV1(
            IEventV1OrchestrationServiceV1 eventV1OrchestrationServiceV1,
            IEventArchiveV1OrchestrationService eventArchiveV1OrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV1OrchestrationServiceV1 = eventV1OrchestrationServiceV1;
            this.eventArchiveV1OrchestrationService = eventArchiveV1OrchestrationService;
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
                EventArchiveV1 eventArchiveV1 =
                    await MapToEventArchiveV1Async(eventV1);

                await this.eventArchiveV1OrchestrationService
                    .AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                        eventArchiveV1);

                await this.eventV1OrchestrationServiceV1
                    .RemoveEventV1AndListenerEventV1sAsync(
                        eventV1);
            }
        });

        private async ValueTask<EventArchiveV1> MapToEventArchiveV1Async(
            EventV1 eventV1)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return new EventArchiveV1
            {
                Id = eventV1.Id,
                Content = eventV1.Content,
                Type = (EventArchiveTypeV1)eventV1.Type,
                CreatedDate = eventV1.CreatedDate,
                UpdatedDate = eventV1.UpdatedDate,
                ScheduledDate = eventV1.ScheduledDate,
                ArchivedDate = currentDateTime,
                EventAddressId = eventV1.EventAddressId,

                ListenerEventArchiveV1s = eventV1.ListenerEventV1s
                    ?.Select(listenerEvent =>
                        MapToListenerEventArchiveV1(
                            listenerEvent,
                            currentDateTime))
                                .ToList()
            };
        }

        private ListenerEventArchiveV1 MapToListenerEventArchiveV1(
            ListenerEventV1 listenerEventV1,
            DateTimeOffset currentDateTime)
        {
            return new ListenerEventArchiveV1
            {
                Id = listenerEventV1.Id,
                Status = (ListenerEventArchiveStatusV1)listenerEventV1.Status,
                Response = listenerEventV1.Response,
                ResponseReasonPhrase = listenerEventV1.ResponseReasonPhrase,
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
