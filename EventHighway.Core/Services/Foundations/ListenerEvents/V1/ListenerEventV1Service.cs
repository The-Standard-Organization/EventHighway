// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Services.Foundations.ListernEvents.V1
{
    internal partial class ListenerEventV1Service : IListenerEventV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV1> AddListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventV1OnAddAsync(listenerEventV1);

            return await storageBroker.InsertListenerEventV1Async(listenerEventV1);
        });

        public ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventV1sAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllListenerEventsV1Async());

        public ValueTask<ListenerEventV1> ModifyListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventV1OnModifyAsync(listenerEventV1);

            ListenerEventV1 maybeListenerEventV1 =
                await this.storageBroker.SelectListenerEventByIdV1Async(
                    listenerEventV1.Id);

            ValidateListenerEventV1AgainstStorage(listenerEventV1, maybeListenerEventV1);

            return await storageBroker.UpdateListenerEventV1Async(listenerEventV1);
        });

        public ValueTask<ListenerEventV1> RemoveListenerEventV1ByIdAsync(Guid listenerEventV1Id) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV1Id(listenerEventV1Id);

            ListenerEventV1 maybeListenerEventV1 =
                await this.storageBroker.SelectListenerEventByIdV1Async(listenerEventV1Id);

            ValidateListenerEventV1Exists(maybeListenerEventV1, listenerEventV1Id);

            return await this.storageBroker.DeleteListenerEventV1Async(maybeListenerEventV1);
        });
    }
}
