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
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2Service : IListenerEventArchiveV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchiveV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventArchiveV2> AddListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateListenerEventArchiveV2OnAddAsync(listenerEventArchiveV2);

            return await this.storageBroker.InsertListenerEventArchiveV2Async(
                listenerEventArchiveV2,
                cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllListenerEventArchiveV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker
                .SelectAllListenerEventArchiveV2sWithEventListenerV2Async(cancellationToken);
        });

        public ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveListenerEventArchiveV2sByQueryAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<IEnumerable<ListenerEventArchiveV2>> BulkAddListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventArchiveV2sIsNotNull(listenerEventArchiveV2s);

            IEnumerable<Guid> incomingIds =
                listenerEventArchiveV2s.Select(listenerEventArchiveV2 => listenerEventArchiveV2.Id).ToList();

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                await this.storageBroker.SelectAllListenerEventArchiveV2sAsync(cancellationToken);

            List<ListenerEventArchiveV2> existingItems =
                storedListenerEventArchiveV2s
                    .Where(storedListenerEventArchiveV2 =>
                        incomingIds.Contains(storedListenerEventArchiveV2.Id))
                            .ToList();

            var existingIds = existingItems
                .Select(existingItem => existingItem.Id)
                    .ToHashSet();

            DateTimeOffset archivedDate =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            List<ListenerEventArchiveV2> itemsToBulkAdd = new List<ListenerEventArchiveV2>();

            foreach (ListenerEventArchiveV2 item in listenerEventArchiveV2s
                .Where(listenerEventArchiveV2 => !existingIds.Contains(listenerEventArchiveV2.Id)))
            {
                item.ArchivedDate = archivedDate;

                try
                {
                    await ValidateListenerEventArchiveV2OnAddAsync(item);
                    itemsToBulkAdd.Add(item);
                }
                catch (NullListenerEventArchiveV2Exception nullListenerEventArchiveV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(nullListenerEventArchiveV2Exception);
                }
                catch (InvalidListenerEventArchiveV2Exception invalidListenerEventArchiveV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(invalidListenerEventArchiveV2Exception);
                }
            }

            await this.storageBroker.BulkInsertListenerEventArchiveV2sAsync(itemsToBulkAdd, cancellationToken);

            return existingItems.Concat(itemsToBulkAdd).ToList();
        });

        public ValueTask BulkRemoveListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventArchiveV2sIsNotNull(listenerEventArchiveV2s);

            await this.storageBroker.BulkDeleteListenerEventArchiveV2sAsync(
                listenerEventArchiveV2s, cancellationToken);
        });
    }
}