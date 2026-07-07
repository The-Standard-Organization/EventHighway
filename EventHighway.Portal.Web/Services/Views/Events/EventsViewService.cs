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
            IQueryable<EventV2> events =
                await this.eventHighwayBroker.RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken);

            return events
                .OrderByDescending(@event => @event.CreatedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventView> RetrieveEventByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<EventV2> events =
                await this.eventHighwayBroker.RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken);

            EventV2 @event = events
                .FirstOrDefault(retrievedEvent => retrievedEvent.Id == eventId);

            return @event is null ? null : AsView(@event);
        });

        private static EventView AsView(EventV2 @event) =>
            new EventView
            {
                Id = @event.Id,
                EventName = @event.EventName,
                Content = @event.Content,
                Type = @event.Type.ToString(),
                Status = @event.Status.ToString(),
                EventAddressV2Id = @event.EventAddressV2Id,
                EventAddressName = @event.EventAddressV2?.Name ?? string.Empty,
                EventParticipantV2Id = @event.EventParticipantV2Id,
                ScheduledDate = @event.ScheduledDate,
                CreatedDate = @event.CreatedDate
            };
    }
}
