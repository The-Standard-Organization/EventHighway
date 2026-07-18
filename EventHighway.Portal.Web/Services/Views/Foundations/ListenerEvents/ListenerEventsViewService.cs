// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents;

namespace EventHighway.Portal.Web.Services.Views.Foundations.ListenerEvents
{
    public partial class ListenerEventsViewService : IListenerEventsViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventsViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        private const int RetrievalPageSize = 1000;

        public ValueTask<List<ListenerEventView>> RetrieveAllListenerEventsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventV2> listenerEvents =
                await RetrieveAllPagesAsync(
                    new ListenerEventV2Query { Take = RetrievalPageSize },
                    withEventListener: false,
                    cancellationToken);

            return listenerEvents
                .Select(AsView)
                .ToList();
        });

        public ValueTask<List<ListenerEventView>> RetrieveListenerEventsByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventV2> listenerEvents =
                await RetrieveAllPagesAsync(
                    new ListenerEventV2Query
                    {
                        EventV2Id = eventId,
                        Take = RetrievalPageSize
                    },
                    withEventListener: true,
                    cancellationToken);

            return listenerEvents
                .Select(AsViewWithEventListener)
                .ToList();
        });

        public ValueTask<ListenerEventView?> RetrieveListenerEventByIdAsync(
            Guid listenerEventId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventV2> listenerEvents =
                await RetrieveAllPagesAsync(
                    new ListenerEventV2Query { Take = RetrievalPageSize },
                    withEventListener: false,
                    cancellationToken);

            ListenerEventV2? listenerEvent = listenerEvents
                .FirstOrDefault(retrievedEvent => retrievedEvent.Id == listenerEventId);

            return listenerEvent is null ? null : AsView(listenerEvent);
        });

        private async ValueTask<List<ListenerEventV2>> RetrieveAllPagesAsync(
            ListenerEventV2Query listenerEventV2Query,
            bool withEventListener,
            CancellationToken cancellationToken)
        {
            var listenerEvents = new List<ListenerEventV2>();

            while (true)
            {
                IReadOnlyList<ListenerEventV2> listenerEventPage = withEventListener
                    ? await this.eventHighwayBroker
                        .RetrieveAllListenerEventV2sWithEventListenerV2Async(
                            listenerEventV2Query, cancellationToken)
                    : await this.eventHighwayBroker.RetrieveAllListenerEventV2sAsync(
                        listenerEventV2Query, cancellationToken);

                listenerEvents.AddRange(listenerEventPage);

                if (listenerEventPage.Count < listenerEventV2Query.Take)
                {
                    break;
                }

                listenerEventV2Query.Skip += listenerEventV2Query.Take;
            }

            return listenerEvents;
        }

        public ValueTask<ListenerEventView> RemoveListenerEventByIdAsync(
            Guid listenerEventId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ListenerEventV2 removedListenerEvent =
                await this.eventHighwayBroker.RemoveListenerEventV2ByIdAsync(
                    listenerEventId, cancellationToken);

            return AsView(removedListenerEvent);
        });

        public ValueTask<int> PurgeListenerEventsOlderThanAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventV2> staleListenerEvents =
                (await RetrieveAllPagesAsync(
                    new ListenerEventV2Query
                    {
                        CreatedTo = olderThan,
                        Take = RetrievalPageSize
                    },
                    withEventListener: false,
                    cancellationToken))
                    .Where(listenerEvent => listenerEvent.CreatedDate < olderThan)
                    .ToList();

            foreach (ListenerEventV2 staleListenerEvent in staleListenerEvents)
            {
                await this.eventHighwayBroker.RemoveListenerEventV2ByIdAsync(
                    staleListenerEvent.Id, cancellationToken);
            }

            return staleListenerEvents.Count;
        });

        private static ListenerEventView AsView(ListenerEventV2 listenerEvent) =>
            new ListenerEventView
            {
                Id = listenerEvent.Id,
                Status = listenerEvent.Status.ToString(),
                ResponseCode = listenerEvent.ResponseCode,
                ResponseMessage = listenerEvent.ResponseMessage,
                RemainingRetryAttempts = listenerEvent.RemainingRetryAttempts,
                RetryAttemptsAllowed = listenerEvent.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = listenerEvent.NextRetryAttemptNotBefore,
                DispatchedDate = listenerEvent.DispatchedDate,
                EventV2Id = listenerEvent.EventV2Id,
                EventAddressV2Id = listenerEvent.EventAddressV2Id,
                EventListenerV2Id = listenerEvent.EventListenerV2Id,
                EventParticipantV2Id = listenerEvent.EventParticipantV2Id,
                CreatedDate = listenerEvent.CreatedDate
            };

        private static ListenerEventView AsViewWithEventListener(ListenerEventV2 listenerEvent) =>
            new ListenerEventView
            {
                Id = listenerEvent.Id,
                Status = listenerEvent.Status.ToString(),
                ResponseCode = listenerEvent.ResponseCode,
                ResponseMessage = listenerEvent.ResponseMessage,
                RemainingRetryAttempts = listenerEvent.RemainingRetryAttempts,
                RetryAttemptsAllowed = listenerEvent.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = listenerEvent.NextRetryAttemptNotBefore,
                DispatchedDate = listenerEvent.DispatchedDate,
                EventV2Id = listenerEvent.EventV2Id,
                EventAddressV2Id = listenerEvent.EventAddressV2Id,
                EventListenerV2Id = listenerEvent.EventListenerV2Id,
                ListenerName = listenerEvent.EventListenerV2?.Name,
                EventParticipantV2Id = listenerEvent.EventParticipantV2Id,
                CreatedDate = listenerEvent.CreatedDate
            };
    }
}
