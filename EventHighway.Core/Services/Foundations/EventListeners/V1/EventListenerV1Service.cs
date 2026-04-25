// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Services.Foundations.EventListeners.V1
{
    internal partial class EventListenerV1Service : IEventListenerV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventListenerV1> AddEventListenerV1Async(EventListenerV1 eventListenerV1) =>
        TryCatch(async () =>
        {
            await ValidateEventListenerV1OnAddAsync(eventListenerV1);

            return await this.storageBroker.InsertEventListenerV1Async(eventListenerV1);
        });

        public ValueTask<IQueryable<EventListenerV1>> RetrieveAllEventListenerV1sAsync() =>
        TryCatch(async () => await storageBroker.SelectAllEventListenersV1Async());

        public ValueTask<EventListenerV1> RemoveEventListenerV1ByIdAsync(Guid eventListenerV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV1Id(eventListenerV1Id);

            EventListenerV1 maybeEventListenerV1 =
                await this.storageBroker.SelectEventListenerByIdV1Async(eventListenerV1Id);

            ValidateEventListenerV1Exists(maybeEventListenerV1, eventListenerV1Id);

            return await this.storageBroker.DeleteEventListenerV1Async(maybeEventListenerV1);
        });
    }
}
