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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Core.Services.Foundations.EventArchives.V2
{
    internal partial class EventArchiveV2Service : IEventArchiveV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventArchiveV2> AddEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await ValidateEventArchiveV2OnAddAsync(eventArchiveV2);

            return await this.storageBroker.InsertEventArchiveV2Async(eventArchiveV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectAllEventArchiveV2sAsync();
        });

        public ValueTask<IQueryable<EventArchiveV2>>
            RetrieveAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker
                .SelectAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync();
        });

        public ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2Id(eventArchiveV2Id);

            return await this.storageBroker.SelectEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);
        });

        public ValueTask<EventArchiveV2> RemoveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2Id(eventArchiveV2Id);

            EventArchiveV2 maybeEventArchiveV2 =
                await this.storageBroker.SelectEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);

            ValidateEventArchiveV2Exists(maybeEventArchiveV2, eventArchiveV2Id);

            return await this.storageBroker.DeleteEventArchiveV2Async(maybeEventArchiveV2, cancellationToken);
        });

        public ValueTask<IEnumerable<EventArchiveV2>> BulkAddEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);
            List<EventArchiveV2> validItems = new List<EventArchiveV2>();

            DateTimeOffset archivedDate =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            foreach (EventArchiveV2 item in eventArchiveV2s)
            {
                item.ArchivedDate = archivedDate;

                try
                {
                    await ValidateEventArchiveV2OnAddAsync(item);
                    validItems.Add(item);
                }
                catch (Exception)
                { }
            }

            await this.storageBroker.BulkInsertEventArchiveV2sAsync(validItems, cancellationToken);

            return validItems;
        });

        public ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            await this.storageBroker.BulkDeleteEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);
        });
    }
}