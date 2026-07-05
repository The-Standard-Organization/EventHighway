// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventArchives;

namespace EventHighway.Portal.Web.Services.Views.EventArchives
{
    public partial class EventArchivesViewService : IEventArchivesViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventArchivesViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveProcessedEventsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.eventHighwayBroker.ArchiveEventV2sAsync(cancellationToken);
        });

        public ValueTask PurgeArchivesOlderThanAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.eventHighwayBroker.PurgeEventArchiveV2sAsync(
                olderThan, cancellationToken);
        });

        public ValueTask<List<EventArchiveView>> RetrieveAllEventArchivesAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<EventArchiveV2> eventArchives =
                await this.eventHighwayBroker
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(cancellationToken);

            return eventArchives
                .OrderByDescending(eventArchive => eventArchive.ArchivedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventArchiveView> RetrieveEventArchiveByIdAsync(
            Guid eventArchiveId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<EventArchiveV2> eventArchives =
                await this.eventHighwayBroker
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(cancellationToken);

            EventArchiveV2 eventArchive = eventArchives
                .FirstOrDefault(retrievedArchive => retrievedArchive.Id == eventArchiveId);

            return eventArchive is null ? null : AsView(eventArchive);
        });

        private static EventArchiveView AsView(EventArchiveV2 eventArchive) =>
            new EventArchiveView
            {
                Id = eventArchive.Id,
                EventName = eventArchive.EventName,
                Content = eventArchive.Content,
                Type = eventArchive.Type.ToString(),
                Status = eventArchive.Status.ToString(),
                EventAddressV2Id = eventArchive.EventAddressV2Id,
                EventAddressName = eventArchive.EventAddressV2?.Name ?? string.Empty,
                EventParticipantV2Id = eventArchive.EventParticipantV2Id,
                ScheduledDate = eventArchive.ScheduledDate,
                CreatedDate = eventArchive.CreatedDate,
                ArchivedDate = eventArchive.ArchivedDate
            };
    }
}
