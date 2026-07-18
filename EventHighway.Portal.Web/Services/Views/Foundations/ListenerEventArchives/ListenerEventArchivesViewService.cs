// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives;

namespace EventHighway.Portal.Web.Services.Views.Foundations.ListenerEventArchives
{
    public partial class ListenerEventArchivesViewService : IListenerEventArchivesViewService
    {
        private const int RetrievalPageSize = 1000;

        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchivesViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<ListenerEventArchiveView>> RetrieveAllListenerEventArchivesAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventArchiveV2> listenerEventArchives =
                await RetrieveAllPagesAsync(
                    new ListenerEventArchiveV2Query { Take = RetrievalPageSize },
                    withEventListener: false,
                    cancellationToken);

            return listenerEventArchives
                .OrderByDescending(listenerEventArchive => listenerEventArchive.CreatedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<List<ListenerEventArchiveView>>
            RetrieveListenerEventArchivesByEventArchiveIdAsync(
                Guid eventArchiveId,
                CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventArchiveV2> listenerEventArchives =
                await RetrieveAllPagesAsync(
                    new ListenerEventArchiveV2Query
                    {
                        EventArchiveV2Id = eventArchiveId,
                        Take = RetrievalPageSize
                    },
                    withEventListener: true,
                    cancellationToken);

            return listenerEventArchives
                .OrderByDescending(listenerEventArchive => listenerEventArchive.CreatedDate)
                .Select(AsViewWithEventListener)
                .ToList();
        });

        public ValueTask<ListenerEventArchiveView?> RetrieveListenerEventArchiveByIdAsync(
            Guid listenerEventArchiveId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            List<ListenerEventArchiveV2> listenerEventArchives =
                await RetrieveAllPagesAsync(
                    new ListenerEventArchiveV2Query { Take = RetrievalPageSize },
                    withEventListener: false,
                    cancellationToken);

            ListenerEventArchiveV2? listenerEventArchive = listenerEventArchives
                .FirstOrDefault(retrievedArchive => retrievedArchive.Id == listenerEventArchiveId);

            return listenerEventArchive is null ? null : AsView(listenerEventArchive);
        });

        private async ValueTask<List<ListenerEventArchiveV2>> RetrieveAllPagesAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            bool withEventListener,
            CancellationToken cancellationToken)
        {
            var listenerEventArchives = new List<ListenerEventArchiveV2>();

            while (true)
            {
                IReadOnlyList<ListenerEventArchiveV2> listenerEventArchivePage = withEventListener
                    ? await this.eventHighwayBroker
                        .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                            listenerEventArchiveV2Query, cancellationToken)
                    : await this.eventHighwayBroker.RetrieveAllListenerEventArchiveV2sAsync(
                        listenerEventArchiveV2Query, cancellationToken);

                listenerEventArchives.AddRange(listenerEventArchivePage);

                if (listenerEventArchivePage.Count < listenerEventArchiveV2Query.Take)
                {
                    break;
                }

                listenerEventArchiveV2Query.Skip += listenerEventArchiveV2Query.Take;
            }

            return listenerEventArchives;
        }

        private static ListenerEventArchiveView AsView(
            ListenerEventArchiveV2 listenerEventArchive) =>
            new ListenerEventArchiveView
            {
                Id = listenerEventArchive.Id,
                Status = listenerEventArchive.Status.ToString(),
                Response = listenerEventArchive.Response,
                ResponseCode = listenerEventArchive.ResponseCode,
                ResponseMessage = listenerEventArchive.ResponseMessage,
                RemainingRetryAttempts = listenerEventArchive.RemainingRetryAttempts,
                RetryAttemptsAllowed = listenerEventArchive.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = listenerEventArchive.NextRetryAttemptNotBefore,
                DispatchedDate = listenerEventArchive.DispatchedDate,
                EventV2Id = listenerEventArchive.EventV2Id,
                EventAddressV2Id = listenerEventArchive.EventAddressV2Id,
                EventListenerV2Id = listenerEventArchive.EventListenerV2Id,
                EventArchiveV2Id = listenerEventArchive.EventArchiveV2Id,
                EventParticipantV2Id = listenerEventArchive.EventParticipantV2Id,
                CreatedDate = listenerEventArchive.CreatedDate,
                ArchivedDate = listenerEventArchive.ArchivedDate
            };

        private static ListenerEventArchiveView AsViewWithEventListener(
            ListenerEventArchiveV2 listenerEventArchive) =>
            new ListenerEventArchiveView
            {
                Id = listenerEventArchive.Id,
                Status = listenerEventArchive.Status.ToString(),
                Response = listenerEventArchive.Response,
                ResponseCode = listenerEventArchive.ResponseCode,
                ResponseMessage = listenerEventArchive.ResponseMessage,
                RemainingRetryAttempts = listenerEventArchive.RemainingRetryAttempts,
                RetryAttemptsAllowed = listenerEventArchive.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = listenerEventArchive.NextRetryAttemptNotBefore,
                DispatchedDate = listenerEventArchive.DispatchedDate,
                EventV2Id = listenerEventArchive.EventV2Id,
                EventAddressV2Id = listenerEventArchive.EventAddressV2Id,
                EventListenerV2Id = listenerEventArchive.EventListenerV2Id,
                ListenerName = listenerEventArchive.EventListenerV2?.Name,
                EventArchiveV2Id = listenerEventArchive.EventArchiveV2Id,
                EventParticipantV2Id = listenerEventArchive.EventParticipantV2Id,
                CreatedDate = listenerEventArchive.CreatedDate,
                ArchivedDate = listenerEventArchive.ArchivedDate
            };
    }
}
