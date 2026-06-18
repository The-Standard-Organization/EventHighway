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
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal partial class EventListenerArchiveV2Service : IEventListenerArchiveV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerArchiveV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<EventListenerArchiveV2> AddEventListenerArchiveV2Async(
            EventListenerArchiveV2 eventListenerArchiveV2,
            CancellationToken cancellationToken = default) =>
            await this.storageBroker.InsertEventListenerArchiveV2Async(
                eventListenerArchiveV2,
                cancellationToken);

        public ValueTask<IQueryable<EventListenerArchiveV2>> RetrieveAllEventListenerArchiveV2sAsync() =>
            throw new NotImplementedException();

        public ValueTask<IEnumerable<EventListenerArchiveV2>> BulkAddEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask BulkRemoveEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
