// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.Events;

namespace EventHighway.Portal.Web.Services.Views.Events
{
    public partial class EventsViewService : IEventsViewService
    {
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
            IQueryable<EventV2> events =
                await this.eventHighwayBroker.RetrieveAllEventV2sAsync(cancellationToken);

            return events.Count(@event => @event.Status == EventStatusV2.Quarantined);
        });

        public ValueTask<List<EventView>> RetrieveAllEventsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<EventV2Summary> eventSummaries =
                await this.eventHighwayBroker.RetrieveAllEventV2SummariesAsync(cancellationToken);

            return eventSummaries
                .OrderByDescending(eventSummary => eventSummary.CreatedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventView> RetrieveEventByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventV2Summary eventSummary =
                await this.eventHighwayBroker.RetrieveEventV2SummaryByIdAsync(
                    eventId, cancellationToken);

            return eventSummary is null ? null : AsView(eventSummary);
        });

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
