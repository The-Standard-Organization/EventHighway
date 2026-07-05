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
            throw new NotImplementedException();
    }
}
