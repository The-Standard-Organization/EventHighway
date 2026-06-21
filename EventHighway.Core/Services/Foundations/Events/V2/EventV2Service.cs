// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Foundations.Events.V2
{
    internal partial class EventV2Service : IEventV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IJsonBroker jsonBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            IJsonBroker jsonBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.jsonBroker = jsonBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV2> AddEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventV2OnAddAsync(eventV2);

            return await storageBroker.InsertEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventV2sAsync(cancellationToken);
        });

        public ValueTask<EventV2> ModifyEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventV2OnModifyAsync(eventV2);

            EventV2 maybeEventV2 =
                await this.storageBroker.SelectEventV2ByIdAsync(
                    eventV2.Id, cancellationToken);

            ValidateEventV2AgainstStorage(eventV2, maybeEventV2);

            return await storageBroker.UpdateEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            EventV2 maybeEventV2 =
                await this.storageBroker.SelectEventV2ByIdAsync(eventV2Id, cancellationToken);

            ValidateEventV2Exists(maybeEventV2, eventV2Id);

            return await this.storageBroker.DeleteEventV2Async(maybeEventV2, cancellationToken);
        });

        public ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2sIsNotNull(eventV2s);

            await this.storageBroker.BulkDeleteEventV2sAsync(eventV2s, cancellationToken);
        });

        public ValueTask<string> RemoveVolatilePathsAsync(
            string content,
            string[] volatileContentPaths) =>
                throw new System.NotImplementedException();
    }
}
