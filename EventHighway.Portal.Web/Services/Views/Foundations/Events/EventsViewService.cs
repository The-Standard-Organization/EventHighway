// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Events;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Events
{
    public partial class EventsViewService : IEventsViewService
    {
        private const int RetrievalPageSize = 1000;

        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventsViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<int> RetrieveArchivableEventCountAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IReadOnlyList<EventV2> quarantinedEvents =
                await this.eventHighwayBroker.RetrieveAllEventV2sAsync(
                    new EventV2Query
                    {
                        Status = EventStatusV2.Quarantined,
                        Take = RetrievalPageSize
                    },
                    cancellationToken);

            return quarantinedEvents.Count;
        });

        public ValueTask<List<EventView>> RetrieveAllEventsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<EventV2Summary> eventSummaries =
                await ComputeEventV2SummariesAsync(cancellationToken);

            return eventSummaries
                .OrderByDescending(eventSummary => eventSummary.CreatedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventView?> RetrieveEventByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<EventV2Summary> eventSummaries =
                await ComputeEventV2SummariesAsync(cancellationToken);

            EventV2Summary? eventSummary =
                eventSummaries.FirstOrDefault(summary => summary.Id == eventId);

            return eventSummary is null ? null : AsView(eventSummary);
        });

        private async ValueTask<List<EventV2Summary>> ComputeEventV2SummariesAsync(
            CancellationToken cancellationToken)
        {
            var eventV2Query = new EventV2Query { Take = RetrievalPageSize };
            var events = new List<EventV2>();

            while (true)
            {
                IReadOnlyList<EventV2> eventV2Page =
                    await this.eventHighwayBroker.RetrieveAllEventV2sWithEventAddressV2Async(
                        eventV2Query, cancellationToken);

                events.AddRange(eventV2Page);

                if (eventV2Page.Count < eventV2Query.Take)
                {
                    break;
                }

                eventV2Query.Skip += eventV2Query.Take;
            }

            ILookup<Guid, ListenerEventV2> listenerEventsByEventId =
                (await this.eventHighwayBroker.RetrieveAllListenerEventV2sAsync(
                    cancellationToken))
                    .ToLookup(listenerEvent => listenerEvent.EventV2Id);

            return events
                .Select(@event => AsEventV2Summary(@event, listenerEventsByEventId))
                .ToList();
        }

        private static EventV2Summary AsEventV2Summary(
            EventV2 @event,
            ILookup<Guid, ListenerEventV2> listenerEventsByEventId) =>
            new EventV2Summary
            {
                Id = @event.Id,
                EventName = @event.EventName,
                Content = @event.Content,
                Type = @event.Type,
                Status = @event.Status,
                EventAddressV2Id = @event.EventAddressV2Id,

                EventAddressName =
                    @event.EventAddressV2 != null ? @event.EventAddressV2.Name : null,

                EventParticipantV2Id = @event.EventParticipantV2Id,
                ScheduledDate = @event.ScheduledDate,
                CreatedDate = @event.CreatedDate,
                ListenerEventCount = listenerEventsByEventId[@event.Id].Count(),

                SucceededListenerEventCount = listenerEventsByEventId[@event.Id].Count(
                    listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success)
            };

        private static EventView AsView(EventV2Summary eventSummary) =>
            new EventView
            {
                Id = eventSummary.Id,
                EventName = eventSummary.EventName ?? string.Empty,
                Content = eventSummary.Content ?? string.Empty,
                Type = eventSummary.Type.ToString(),
                Status = eventSummary.Status.ToString(),
                EventAddressV2Id = eventSummary.EventAddressV2Id,
                EventAddressName = eventSummary.EventAddressName ?? string.Empty,
                EventParticipantV2Id = eventSummary.EventParticipantV2Id,
                ScheduledDate = eventSummary.ScheduledDate,
                CreatedDate = eventSummary.CreatedDate,
                ListenerEventCount = eventSummary.ListenerEventCount,
                SucceededListenerEventCount = eventSummary.SucceededListenerEventCount
            };
    }
}
