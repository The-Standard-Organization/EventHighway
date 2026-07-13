// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventArchives
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
            List<EventArchiveV2Summary> eventArchiveSummaries =
                await this.eventHighwayBroker.RetrieveAllEventArchiveV2SummariesAsync(
                    cancellationToken);

            return eventArchiveSummaries
                .OrderByDescending(eventArchiveSummary => eventArchiveSummary.ArchivedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventArchiveView> RetrieveEventArchiveByIdAsync(
            Guid eventArchiveId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventArchiveV2Summary eventArchiveSummary =
                await this.eventHighwayBroker.RetrieveEventArchiveV2SummaryByIdAsync(
                    eventArchiveId, cancellationToken);

            return eventArchiveSummary is null ? null : AsView(eventArchiveSummary);
        });

        private static EventArchiveView AsView(EventArchiveV2Summary eventArchiveSummary) =>
            new EventArchiveView
            {
                Id = eventArchiveSummary.Id,
                EventName = eventArchiveSummary.EventName ?? string.Empty,
                Content = eventArchiveSummary.Content ?? string.Empty,
                Type = eventArchiveSummary.Type.ToString(),
                Status = eventArchiveSummary.Status.ToString(),
                EventAddressV2Id = eventArchiveSummary.EventAddressV2Id,
                EventAddressName = eventArchiveSummary.EventAddressName ?? string.Empty,
                EventParticipantV2Id = eventArchiveSummary.EventParticipantV2Id,
                ScheduledDate = eventArchiveSummary.ScheduledDate,
                CreatedDate = eventArchiveSummary.CreatedDate,
                ArchivedDate = eventArchiveSummary.ArchivedDate,
                ListenerEventCount = eventArchiveSummary.ListenerEventCount,
                SucceededListenerEventCount = eventArchiveSummary.SucceededListenerEventCount
            };
    }
}
