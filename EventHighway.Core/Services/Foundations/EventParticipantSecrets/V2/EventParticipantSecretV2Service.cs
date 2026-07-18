// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Hashings;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service : IEventParticipantSecretV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IHashBroker hashBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantSecretV2Service(
            IStorageBroker storageBroker,
            IHashBroker hashBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.hashBroker = hashBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventParticipantSecretV2OnAddAsync(eventParticipantSecretV2);

            eventParticipantSecretV2.Secret =
                this.hashBroker.GenerateSha256Hash(eventParticipantSecretV2.Secret);

            return await this.storageBroker.InsertEventParticipantSecretV2Async(
                eventParticipantSecretV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventParticipantSecretV2>> RetrieveAllEventParticipantSecretV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventParticipantSecretV2sAsync(
                cancellationToken);
        });

        public ValueTask<IReadOnlyList<EventParticipantSecretV2>> RetrieveEventParticipantSecretV2sByQueryAsync(
            EventParticipantSecretV2Query eventParticipantSecretV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantSecretV2Query(eventParticipantSecretV2Query);

            IQueryable<EventParticipantSecretV2> eventParticipantSecretV2s =
                await this.storageBroker.SelectAllEventParticipantSecretV2sAsync(cancellationToken);

            return ApplyEventParticipantSecretV2Query(
                eventParticipantSecretV2s, eventParticipantSecretV2Query);
        });

        private static IReadOnlyList<EventParticipantSecretV2> ApplyEventParticipantSecretV2Query(
            IQueryable<EventParticipantSecretV2> eventParticipantSecretV2s,
            EventParticipantSecretV2Query eventParticipantSecretV2Query)
        {
            if (eventParticipantSecretV2Query.EventParticipantV2Id is not null)
            {
                eventParticipantSecretV2s = eventParticipantSecretV2s.Where(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.EventParticipantV2Id
                        == eventParticipantSecretV2Query.EventParticipantV2Id);
            }

            if (eventParticipantSecretV2Query.IsActive is not null)
            {
                eventParticipantSecretV2s = eventParticipantSecretV2s.Where(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.IsActive == eventParticipantSecretV2Query.IsActive);
            }

            if (eventParticipantSecretV2Query.CreatedFrom is not null)
            {
                eventParticipantSecretV2s = eventParticipantSecretV2s.Where(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.CreatedDate >= eventParticipantSecretV2Query.CreatedFrom);
            }

            if (eventParticipantSecretV2Query.CreatedTo is not null)
            {
                eventParticipantSecretV2s = eventParticipantSecretV2s.Where(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.CreatedDate <= eventParticipantSecretV2Query.CreatedTo);
            }

            return eventParticipantSecretV2s
                .OrderByDescending(eventParticipantSecretV2 => eventParticipantSecretV2.CreatedDate)
                .ThenBy(eventParticipantSecretV2 => eventParticipantSecretV2.Id)
                .Skip(eventParticipantSecretV2Query.Skip)
                .Take(eventParticipantSecretV2Query.Take)
                .ToList();
        }

        public ValueTask<EventParticipantSecretV2> RetrieveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventParticipantSecretV2Function(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantSecretV2Id(eventParticipantSecretV2Id);

            EventParticipantSecretV2 maybeEventParticipantSecretV2 =
                await this.storageBroker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2Id, cancellationToken);

            ValidateEventParticipantSecretV2Exists(maybeEventParticipantSecretV2, eventParticipantSecretV2Id);

            return maybeEventParticipantSecretV2;
        }));

        public ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventParticipantSecretV2Function(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventParticipantSecretV2OnModifyAsync(eventParticipantSecretV2);

            EventParticipantSecretV2 maybeEventParticipantSecretV2 =
                await this.storageBroker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2.Id, cancellationToken);

            ValidateEventParticipantSecretV2AgainstStorage(eventParticipantSecretV2, maybeEventParticipantSecretV2);

            return await this.storageBroker.UpdateEventParticipantSecretV2Async(
                eventParticipantSecretV2, cancellationToken);
        }));

        public ValueTask<EventParticipantSecretV2> RemoveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventParticipantSecretV2Function(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantSecretV2Id(eventParticipantSecretV2Id);

            EventParticipantSecretV2 maybeEventParticipantSecretV2 =
                await this.storageBroker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2Id, cancellationToken);

            ValidateEventParticipantSecretV2Exists(maybeEventParticipantSecretV2, eventParticipantSecretV2Id);

            return await this.storageBroker.DeleteEventParticipantSecretV2Async(
                maybeEventParticipantSecretV2, cancellationToken);
        }));
    }
}
