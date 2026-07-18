// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventArchives
{
    public partial class EventArchivesViewService : IEventArchivesViewService
    {
        private const int RetrievalPageSize = 1000;

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
                await ComputeEventArchiveV2SummariesAsync(cancellationToken);

            return eventArchiveSummaries
                .OrderByDescending(eventArchiveSummary => eventArchiveSummary.ArchivedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<EventArchiveView?> RetrieveEventArchiveByIdAsync(
            Guid eventArchiveId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<EventArchiveV2Summary> eventArchiveSummaries =
                await ComputeEventArchiveV2SummariesAsync(cancellationToken);

            EventArchiveV2Summary? eventArchiveSummary =
                eventArchiveSummaries.FirstOrDefault(summary => summary.Id == eventArchiveId);

            return eventArchiveSummary is null ? null : AsView(eventArchiveSummary);
        });

        private async ValueTask<List<EventArchiveV2Summary>> ComputeEventArchiveV2SummariesAsync(
            CancellationToken cancellationToken)
        {
            var eventArchiveV2Query = new EventArchiveV2Query { Take = RetrievalPageSize };
            var eventArchives = new List<EventArchiveV2>();

            while (true)
            {
                IReadOnlyList<EventArchiveV2> eventArchivePage =
                    await this.eventHighwayBroker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        eventArchiveV2Query, cancellationToken);

                eventArchives.AddRange(eventArchivePage);

                if (eventArchivePage.Count < eventArchiveV2Query.Take)
                {
                    break;
                }

                eventArchiveV2Query.Skip += eventArchiveV2Query.Take;
            }

            var listenerEventArchiveV2Query =
                new ListenerEventArchiveV2Query { Take = RetrievalPageSize };

            var listenerEventArchives = new List<ListenerEventArchiveV2>();

            while (true)
            {
                IReadOnlyList<ListenerEventArchiveV2> listenerEventArchivePage =
                    await this.eventHighwayBroker.RetrieveAllListenerEventArchiveV2sAsync(
                        listenerEventArchiveV2Query, cancellationToken);

                listenerEventArchives.AddRange(listenerEventArchivePage);

                if (listenerEventArchivePage.Count < listenerEventArchiveV2Query.Take)
                {
                    break;
                }

                listenerEventArchiveV2Query.Skip += listenerEventArchiveV2Query.Take;
            }

            ILookup<Guid, ListenerEventArchiveV2> listenerEventArchivesByEventArchiveId =
                listenerEventArchives.ToLookup(
                    listenerEventArchive => listenerEventArchive.EventArchiveV2Id);

            return eventArchives
                .Select(eventArchive =>
                    AsEventArchiveV2Summary(eventArchive, listenerEventArchivesByEventArchiveId))
                .ToList();
        }

        private static EventArchiveV2Summary AsEventArchiveV2Summary(
            EventArchiveV2 eventArchive,
            ILookup<Guid, ListenerEventArchiveV2> listenerEventArchivesByEventArchiveId) =>
            new EventArchiveV2Summary
            {
                Id = eventArchive.Id,
                EventName = eventArchive.EventName,
                Content = eventArchive.Content,
                Type = eventArchive.Type,
                Status = eventArchive.Status,
                EventAddressV2Id = eventArchive.EventAddressV2Id,

                EventAddressName =
                    eventArchive.EventAddressV2 != null ? eventArchive.EventAddressV2.Name : null,

                EventParticipantV2Id = eventArchive.EventParticipantV2Id,
                ScheduledDate = eventArchive.ScheduledDate,
                CreatedDate = eventArchive.CreatedDate,
                ArchivedDate = eventArchive.ArchivedDate,
                ListenerEventCount = listenerEventArchivesByEventArchiveId[eventArchive.Id].Count(),

                SucceededListenerEventCount =
                    listenerEventArchivesByEventArchiveId[eventArchive.Id].Count(
                        listenerEventArchive =>
                            listenerEventArchive.Status == ListenerEventArchiveStatusV2.Success)
            };

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
