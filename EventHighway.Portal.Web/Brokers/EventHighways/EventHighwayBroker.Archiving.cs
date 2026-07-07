// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // Server-side EF projection: the listener-event counts translate to correlated COUNT
        // subqueries, so no listener-event rows are materialized regardless of volume. The
        // projection MUST run inside the database gate — the gate materializes results before
        // returning, so any navigation access after the gate would hit detached entities.
        private static readonly
            Expression<Func<EventArchiveV2, EventArchiveV2Summary>> AsEventArchiveV2Summary =
                eventArchive => new EventArchiveV2Summary
                {
                    Id = eventArchive.Id,
                    EventName = eventArchive.EventName,
                    Content = eventArchive.Content,
                    Type = eventArchive.Type,
                    Status = eventArchive.Status,
                    EventAddressV2Id = eventArchive.EventAddressV2Id,

                    EventAddressName = eventArchive.EventAddressV2 != null
                        ? eventArchive.EventAddressV2.Name
                        : null,

                    EventParticipantV2Id = eventArchive.EventParticipantV2Id,
                    ScheduledDate = eventArchive.ScheduledDate,
                    CreatedDate = eventArchive.CreatedDate,
                    ArchivedDate = eventArchive.ArchivedDate,
                    ListenerEventCount = eventArchive.ListenerEventArchiveV2s.Count(),

                    SucceededListenerEventCount = eventArchive.ListenerEventArchiveV2s.Count(
                        listenerEventArchive =>
                            listenerEventArchive.Status == ListenerEventArchiveStatusV2.Success)
                };

        public ValueTask ArchiveEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ArchivingEventV2Client.ArchiveEventV2sAsync(cancellationToken),
                cancellationToken);

        public ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ArchivingEventV2Client.PurgeEventArchiveV2sAsync(
                    olderThan, cancellationToken),
                cancellationToken);

        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventArchiveV2Client
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);

        public ValueTask<List<EventArchiveV2Summary>> RetrieveAllEventArchiveV2SummariesAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventArchiveV2Client
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken))
                    .Select(AsEventArchiveV2Summary)
                    .ToList(),
                cancellationToken);

        public ValueTask<EventArchiveV2Summary?> RetrieveEventArchiveV2SummaryByIdAsync(
            Guid eventArchiveId,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventArchiveV2Client
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken))
                    .Where(eventArchive => eventArchive.Id == eventArchiveId)
                    .Select(AsEventArchiveV2Summary)
                    .FirstOrDefault(),
                cancellationToken);
    }
}
