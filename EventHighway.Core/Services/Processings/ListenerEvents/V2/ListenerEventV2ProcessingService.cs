// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V2
{
    internal partial class ListenerEventV2ProcessingService : IListenerEventV2ProcessingService
    {
        private readonly IListenerEventV2Service listenerEventV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV2ProcessingService(
            IListenerEventV2Service listenerEventV2Service,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventV2Service = listenerEventV2Service;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV2> AddListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2IsNotNull(listenerEventV2);

            return await this.listenerEventV2Service
                .AddListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.listenerEventV2Service.RetrieveAllListenerEventV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.listenerEventV2Service
                .RetrieveAllListenerEventV2sWithEventListenerV2Async(cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventV2>> BulkRestoreListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2sIsNotNull(listenerEventV2s);

            return await this.listenerEventV2Service.BulkRestoreListenerEventV2sAsync(
                listenerEventV2s, cancellationToken);
        });

        public ValueTask<ListenerEventV2> ModifyListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2IsNotNull(listenerEventV2);

            return await this.listenerEventV2Service
                .ModifyListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            return await this.listenerEventV2Service
                .RemoveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
        });

        public ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2sIsNotNull(listenerEventV2s);

            await this.listenerEventV2Service
                .BulkRemoveListenerEventV2sAsync(listenerEventV2s, cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfReplayListenerEventV2sAsync(
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveBatchOfReplayListenerEventV2s(take);

            return await this.listenerEventV2Service
                .RetrieveReplayBatchListenerEventV2sWithEventWithEventListenerAsync(
                    take, cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfListenerEventV2sByEventIdsAsync(
            IEnumerable<Guid> eventIds,
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveBatchOfListenerEventV2sByEventIds(eventIds, take);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveListenerEventV2sByEventIdsAsync(eventIds, cancellationToken);

            return take == 0
                ? listenerEventV2s.AsEnumerable()
                : listenerEventV2s.Take(take).AsEnumerable();
        });

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfRetryListenerEventV2sAsync(
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveBatchOfRetryListenerEventV2s(take);

            return await this.listenerEventV2Service
                .RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                    take, cancellationToken);
        });

        public ValueTask<ListenerEventV2> RetrieveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            return await this.listenerEventV2Service
                .RetrieveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveListenerEventV2sByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventListenerV2Id(eventListenerV2Id);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveListenerEventV2sByEventListenerV2IdAsync(eventListenerV2Id, cancellationToken);

            return listenerEventV2s.AsEnumerable();
        });

        public ValueTask<ListenerEventV2> ResetRetriesForListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            ListenerEventV2 maybeListenerEventV2 =
                await this.listenerEventV2Service
                    .RetrieveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);

            RetryConfiguration retryConfiguration =
                this.configurationBroker.GetRetryConfiguration();

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            maybeListenerEventV2.RetryAttemptsAllowed += retryConfiguration.RetryAttemptsAllowed;
            maybeListenerEventV2.RemainingRetryAttempts += retryConfiguration.RetryAttemptsAllowed;
            maybeListenerEventV2.NextRetryAttemptNotBefore = null;
            maybeListenerEventV2.UpdatedDate = now;

            return await this.listenerEventV2Service
                .ModifyListenerEventV2Async(maybeListenerEventV2, cancellationToken);
        });

        public ValueTask ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventListenerV2Id(eventListenerV2Id);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveListenerEventV2sByEventListenerV2IdAsync(eventListenerV2Id, cancellationToken);

            int batchSize =
                this.configurationBroker.GetBatchConfiguration().BatchSizeForBulkProcessing;

            RetryConfiguration retryConfiguration =
                this.configurationBroker.GetRetryConfiguration();

            IQueryable<ListenerEventV2> errorListenerEventV2s =
                listenerEventV2s
                    .Where(listenerEventV2 =>
                        listenerEventV2.Status == ListenerEventStatusV2.Error)
                    .OrderBy(listenerEventV2 => listenerEventV2.CreatedDate)
                    .ThenBy(listenerEventV2 => listenerEventV2.Id);

            int skip = 0;

            while (true)
            {
                List<ListenerEventV2> batch =
                    batchSize <= 0
                        ? errorListenerEventV2s.Skip(skip).ToList()
                        : errorListenerEventV2s.Skip(skip).Take(batchSize).ToList();

                if (batch.Count == 0)
                {
                    break;
                }

                foreach (ListenerEventV2 listenerEventV2 in batch)
                {
                    listenerEventV2.RetryAttemptsAllowed += retryConfiguration.RetryAttemptsAllowed;
                    listenerEventV2.RemainingRetryAttempts += retryConfiguration.RetryAttemptsAllowed;
                    listenerEventV2.NextRetryAttemptNotBefore = null;
                }

                await this.listenerEventV2Service
                    .BulkModifyListenerEventV2sAsync(batch, cancellationToken);

                if (batchSize <= 0)
                {
                    break;
                }

                skip += batchSize;
            }
        });
    }
}
