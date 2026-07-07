// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // Server-side EF projection: the listener-event counts translate to correlated COUNT
        // subqueries, so no listener-event rows are materialized regardless of volume. The
        // projection MUST run inside the database gate — the gate materializes results before
        // returning, so any navigation access after the gate would hit detached entities.
        private static readonly Expression<Func<EventV2, EventV2Summary>> AsEventV2Summary =
            @event => new EventV2Summary
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
                ListenerEventCount = @event.ListenerEventV2s.Count(),

                SucceededListenerEventCount = @event.ListenerEventV2s.Count(
                    listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success)
            };

        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventV2Client
                    .RetrieveAllEventV2sAsync(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);

        public ValueTask<List<EventV2Summary>> RetrieveAllEventV2SummariesAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventV2Client
                    .RetrieveAllEventV2sAsync(cancellationToken))
                    .Select(AsEventV2Summary)
                    .ToList(),
                cancellationToken);

        public ValueTask<EventV2Summary?> RetrieveEventV2SummaryByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventV2Client
                    .RetrieveAllEventV2sAsync(cancellationToken))
                    .Where(@event => @event.Id == eventId)
                    .Select(AsEventV2Summary)
                    .FirstOrDefault(),
                cancellationToken);
    }
}
