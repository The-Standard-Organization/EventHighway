// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Clients.EventHighways.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // The client materializes retrievals per operation, so summaries are computed in
        // memory: events (with their addresses) plus listener events, joined by event id.
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

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventV2Client
                    .RetrieveAllEventV2sAsync(eventV2Query, cancellationToken))
                    .AsQueryable(),
                cancellationToken);

        public ValueTask<List<EventV2Summary>> RetrieveAllEventV2SummariesAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(
                ComputeEventV2SummariesAsync,
                cancellationToken);

        public ValueTask<EventV2Summary?> RetrieveEventV2SummaryByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await ComputeEventV2SummariesAsync(client))
                    .FirstOrDefault(summary => summary.Id == eventId),
                cancellationToken);

        private static async ValueTask<List<EventV2Summary>> ComputeEventV2SummariesAsync(
            IClientV2 client)
        {
            var eventV2Query = new EventV2Query { Take = 1000 };
            List<EventV2> events = new List<EventV2>();

            while (true)
            {
                IReadOnlyList<EventV2> eventV2Page =
                    await client.EventV2Client
                        .RetrieveAllEventV2sWithEventAddressV2Async(eventV2Query);

                events.AddRange(eventV2Page);

                if (eventV2Page.Count < eventV2Query.Take)
                {
                    break;
                }

                eventV2Query.Skip += eventV2Query.Take;
            }

            ILookup<Guid, ListenerEventV2> listenerEventsByEventId =
                (await client.ListenerEventV2Client
                    .RetrieveAllListenerEventV2sAsync())
                    .ToLookup(listenerEvent => listenerEvent.EventV2Id);

            return events
                .Select(@event => AsEventV2Summary(@event, listenerEventsByEventId))
                .ToList();
        }
    }
}
