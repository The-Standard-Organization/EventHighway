// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ListenerEvents.V2
{
    internal partial class ListenerEventV2OrchestrationService : IListenerEventV2OrchestrationService
    {
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV2OrchestrationService(
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.listenerEventV2ProcessingService
                .RetrieveAllListenerEventV2sAsync(cancellationToken);
        });

        public ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveListenerEventV2sByQueryAsync(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            return ApplyListenerEventV2Query(listenerEventV2s, listenerEventV2Query);
        });

        public ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveListenerEventV2sWithEventListenerV2ByQueryAsync(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sWithEventListenerV2Async(cancellationToken);

            return ApplyListenerEventV2Query(listenerEventV2s, listenerEventV2Query);
        });

        private static IReadOnlyList<ListenerEventV2> ApplyListenerEventV2Query(
            IQueryable<ListenerEventV2> listenerEventV2s,
            ListenerEventV2Query listenerEventV2Query)
        {
            if (listenerEventV2Query.Status is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.Status == listenerEventV2Query.Status);
            }

            if (listenerEventV2Query.EventV2Id is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.EventV2Id == listenerEventV2Query.EventV2Id);
            }

            if (listenerEventV2Query.EventAddressV2Id is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.EventAddressV2Id == listenerEventV2Query.EventAddressV2Id);
            }

            if (listenerEventV2Query.EventListenerV2Id is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.EventListenerV2Id == listenerEventV2Query.EventListenerV2Id);
            }

            if (listenerEventV2Query.EventParticipantV2Id is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.EventParticipantV2Id == listenerEventV2Query.EventParticipantV2Id);
            }

            if (listenerEventV2Query.CorrelationId is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.CorrelationId == listenerEventV2Query.CorrelationId);
            }

            if (listenerEventV2Query.CreatedFrom is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.CreatedDate >= listenerEventV2Query.CreatedFrom);
            }

            if (listenerEventV2Query.CreatedTo is not null)
            {
                listenerEventV2s = listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.CreatedDate <= listenerEventV2Query.CreatedTo);
            }

            return listenerEventV2s
                .OrderByDescending(listenerEventV2 => listenerEventV2.CreatedDate)
                .ThenBy(listenerEventV2 => listenerEventV2.Id)
                .Skip(listenerEventV2Query.Skip)
                .Take(listenerEventV2Query.Take)
                .ToList();
        }

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.listenerEventV2ProcessingService
                .RetrieveAllListenerEventV2sWithEventListenerV2Async(cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfListenerEventV2sByEventIdsAsync(
            IEnumerable<Guid> eventV2Ids,
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveBatchOfListenerEventV2sByEventIds(eventV2Ids, take);

            return await this.listenerEventV2ProcessingService
                .RetrieveBatchOfListenerEventV2sByEventIdsAsync(eventV2Ids, take, cancellationToken);
        });

        public ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnBulkRemoveListenerEventV2s(listenerEventV2s);

            await this.listenerEventV2ProcessingService
                .BulkRemoveListenerEventV2sAsync(listenerEventV2s, cancellationToken);
        });

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            return await this.listenerEventV2ProcessingService
                .RemoveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
        });

        public ValueTask<ListenerEventV2> ResetRetriesForListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            return await this.listenerEventV2ProcessingService
                .ResetRetriesForListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
        });

        public ValueTask ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventListenerV2Id(eventListenerV2Id);

            await this.listenerEventV2ProcessingService
                .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    eventListenerV2Id, cancellationToken);
        });
    }
}
