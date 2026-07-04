// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListenerEvents.V2
{
    internal partial class ListenerEventV2Service : IListenerEventV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV2> AddListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateListenerEventV2OnAddAsync(listenerEventV2);

            return await storageBroker.InsertListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllListenerEventV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker
                .SelectAllEventV2sWithListenerEventV2Async(cancellationToken);
        });

        public ValueTask<ListenerEventV2> ModifyListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateListenerEventV2OnModifyAsync(listenerEventV2);

            ListenerEventV2 maybeListenerEventV2 =
                await this.storageBroker.SelectListenerEventV2ByIdAsync(
                    listenerEventV2.Id,
                    cancellationToken);

            ValidateListenerEventV2AgainstStorage(listenerEventV2, maybeListenerEventV2);

            return await storageBroker.UpdateListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            ListenerEventV2 maybeListenerEventV2 =
                await this.storageBroker.SelectListenerEventV2ByIdAsync(
                    listenerEventV2Id,
                    cancellationToken);

            ValidateListenerEventV2Exists(maybeListenerEventV2, listenerEventV2Id);

            return await this.storageBroker.DeleteListenerEventV2Async(maybeListenerEventV2, cancellationToken);
        });

        public ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2sIsNotNull(listenerEventV2s);

            await this.storageBroker.BulkDeleteListenerEventV2sAsync(listenerEventV2s, cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventV2>> BulkRestoreListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2sIsNotNull(listenerEventV2s);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            List<ListenerEventV2> itemsToBulkRestore = new List<ListenerEventV2>();

            foreach (ListenerEventV2 listenerEventV2 in listenerEventV2s)
            {
                try
                {
                    ValidateListenerEventV2OnRestore(listenerEventV2, now);
                    itemsToBulkRestore.Add(listenerEventV2);
                }
                catch (NullListenerEventV2Exception nullListenerEventV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(nullListenerEventV2Exception);
                }
                catch (InvalidListenerEventV2Exception invalidListenerEventV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(invalidListenerEventV2Exception);
                }
            }

            await this.storageBroker.BulkInsertListenerEventV2sAsync(
                itemsToBulkRestore, cancellationToken);

            return (IEnumerable<ListenerEventV2>)itemsToBulkRestore;
        });

        public ValueTask<IEnumerable<ListenerEventV2>> BulkModifyListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2sIsNotNull(listenerEventV2s);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            List<ListenerEventV2> itemsToBulkModify = new List<ListenerEventV2>();

            foreach (ListenerEventV2 listenerEventV2 in listenerEventV2s)
            {
                try
                {
                    listenerEventV2.UpdatedDate = now;
                    ValidateListenerEventV2OnBulkModify(listenerEventV2, now);
                    itemsToBulkModify.Add(listenerEventV2);
                }
                catch (NullListenerEventV2Exception nullListenerEventV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(nullListenerEventV2Exception);
                }
                catch (InvalidListenerEventV2Exception invalidListenerEventV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(invalidListenerEventV2Exception);
                }
            }

            await this.storageBroker.BulkUpdateListenerEventV2sAsync(
                itemsToBulkModify, cancellationToken);

            return (IEnumerable<ListenerEventV2>)itemsToBulkModify;
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveListenerEventV2sByEventIdsAsync(
            IEnumerable<Guid> eventIds,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventIdsIsNotNull(eventIds);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.storageBroker.SelectAllListenerEventV2sAsync(cancellationToken);

            return listenerEventV2s.Where(
                listenerEvent => eventIds.Contains(listenerEvent.EventV2Id));
        });

        public ValueTask<IEnumerable<ListenerEventV2>>
            RetrieveReplayBatchListenerEventV2sWithEventWithEventListenerAsync(
                int take,
                CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveReplayBatch(take);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.storageBroker
                    .SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                        cancellationToken);

            IQueryable<ListenerEventV2> replayListenerEventV2s =
                listenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.Status == ListenerEventStatusV2.Replay);

            return take == 0
                ? replayListenerEventV2s.ToList()
                : replayListenerEventV2s.Take(take).ToList();
        });

        public ValueTask<IEnumerable<ListenerEventV2>>
            RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                int take,
                CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveRetryBatch(take);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.storageBroker
                    .SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                        cancellationToken);

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            IQueryable<ListenerEventV2> retryListenerEventV2s =
                listenerEventV2s
                    .Where(listenerEventV2 =>
                        listenerEventV2.Status == ListenerEventStatusV2.Error
                        && listenerEventV2.RemainingRetryAttempts > 0
                        && (listenerEventV2.NextRetryAttemptNotBefore == null
                            || listenerEventV2.NextRetryAttemptNotBefore <= now))
                    .OrderBy(listenerEventV2 => listenerEventV2.CreatedDate);

            return take == 0
                ? retryListenerEventV2s.ToList()
                : retryListenerEventV2s.Take(take).ToList();
        });

        public ValueTask<ListenerEventV2> RetrieveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2Id(listenerEventV2Id);

            ListenerEventV2 maybeListenerEventV2 =
                await this.storageBroker.SelectListenerEventV2ByIdAsync(
                    listenerEventV2Id,
                    cancellationToken);

            ValidateListenerEventV2Exists(maybeListenerEventV2, listenerEventV2Id);

            return maybeListenerEventV2;
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveListenerEventV2sByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventListenerV2Id(eventListenerV2Id);

            IQueryable<ListenerEventV2> listenerEventV2s =
                await this.storageBroker.SelectAllListenerEventV2sAsync(cancellationToken);

            return listenerEventV2s.Where(listenerEventV2 =>
                listenerEventV2.EventListenerV2Id == eventListenerV2Id);
        });
    }
}
