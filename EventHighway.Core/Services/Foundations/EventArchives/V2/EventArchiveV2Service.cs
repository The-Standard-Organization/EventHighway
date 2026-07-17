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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;

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
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventArchiveV2OnAddAsync(eventArchiveV2);

            return await this.storageBroker.InsertEventArchiveV2Async(eventArchiveV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventArchiveV2sAsync(cancellationToken);
        });

        public ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveEventArchiveV2sByQueryAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventArchiveV2> eventArchiveV2s =
                await this.storageBroker.SelectAllEventArchiveV2sAsync(cancellationToken);

            return ApplyEventArchiveV2Query(eventArchiveV2s, eventArchiveV2Query);
        });

        public ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventArchiveV2> eventArchiveV2s =
                await this.storageBroker
                    .SelectAllEventArchiveV2sWithEventAddressV2Async(cancellationToken);

            return ApplyEventArchiveV2Query(eventArchiveV2s, eventArchiveV2Query);
        });

        private static IReadOnlyList<EventArchiveV2> ApplyEventArchiveV2Query(
            IQueryable<EventArchiveV2> eventArchiveV2s,
            EventArchiveV2Query eventArchiveV2Query)
        {
            if (eventArchiveV2Query.EventAddressV2Id is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.EventAddressV2Id == eventArchiveV2Query.EventAddressV2Id);
            }

            if (eventArchiveV2Query.EventParticipantV2Id is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.EventParticipantV2Id == eventArchiveV2Query.EventParticipantV2Id);
            }

            if (eventArchiveV2Query.EventName is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.EventName == eventArchiveV2Query.EventName);
            }

            if (eventArchiveV2Query.Status is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.Status == eventArchiveV2Query.Status);
            }

            if (eventArchiveV2Query.Type is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.Type == eventArchiveV2Query.Type);
            }

            if (eventArchiveV2Query.CreatedFrom is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.CreatedDate >= eventArchiveV2Query.CreatedFrom);
            }

            if (eventArchiveV2Query.CreatedTo is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.CreatedDate <= eventArchiveV2Query.CreatedTo);
            }

            if (eventArchiveV2Query.ArchivedFrom is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.ArchivedDate >= eventArchiveV2Query.ArchivedFrom);
            }

            if (eventArchiveV2Query.ArchivedTo is not null)
            {
                eventArchiveV2s = eventArchiveV2s.Where(eventArchiveV2 =>
                    eventArchiveV2.ArchivedDate <= eventArchiveV2Query.ArchivedTo);
            }

            return eventArchiveV2s
                .OrderByDescending(eventArchiveV2 => eventArchiveV2.ArchivedDate)
                .ThenBy(eventArchiveV2 => eventArchiveV2.Id)
                .Skip(eventArchiveV2Query.Skip)
                .Take(eventArchiveV2Query.Take)
                .ToList();
        }

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker
                .SelectAllEventArchiveV2sWithEventAddressV2Async(cancellationToken);
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker
                .SelectAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken);
        });

        public ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2Id(eventArchiveV2Id);

            return await this.storageBroker.SelectEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);
        });

        public ValueTask<EventArchiveV2> RemoveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
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
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            IEnumerable<Guid> incomingIds =
                eventArchiveV2s.Select(eventArchiveV2 => eventArchiveV2.Id).ToList();

            IQueryable<EventArchiveV2> storedEventArchiveV2s =
                await this.storageBroker.SelectAllEventArchiveV2sAsync(cancellationToken);

            List<EventArchiveV2> existingItems =
                storedEventArchiveV2s
                    .Where(storedEventArchiveV2 => incomingIds.Contains(storedEventArchiveV2.Id))
                        .ToList();

            var existingIds = existingItems
                .Select(existingItem => existingItem.Id)
                    .ToHashSet();

            DateTimeOffset archivedDate =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            List<EventArchiveV2> itemsToBulkAdd = new List<EventArchiveV2>();

            foreach (EventArchiveV2 item in eventArchiveV2s
                .Where(eventArchiveV2 => !existingIds.Contains(eventArchiveV2.Id)))
            {
                item.ArchivedDate = archivedDate;

                try
                {
                    await ValidateEventArchiveV2OnAddAsync(item);
                    itemsToBulkAdd.Add(item);
                }
                catch (NullEventArchiveV2Exception nullEventArchiveV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(nullEventArchiveV2Exception);
                }
                catch (InvalidEventArchiveV2Exception invalidEventArchiveV2Exception)
                {
                    await this.loggingBroker.LogErrorAsync(invalidEventArchiveV2Exception);
                }
            }

            await this.storageBroker.BulkInsertEventArchiveV2sAsync(itemsToBulkAdd, cancellationToken);

            return (IEnumerable<EventArchiveV2>)existingItems.Concat(itemsToBulkAdd).ToList();
        });

        public ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            await this.storageBroker.BulkDeleteEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);
        });
    }
}