// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;

namespace EventHighway.Core.Services.Foundations.ListenerEvents
{
    internal class ListenerEventService : IListenerEventService
    {
        private readonly IStorageBroker storageBroker;

        public ListenerEventService(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        public async ValueTask<ListenerEvent> AddListenerEventAsync(ListenerEvent listenerEvent) =>
            await storageBroker.InsertListenerEventAsync(listenerEvent);

        public async ValueTask<ListenerEvent> ModifyListenerEventAsync(ListenerEvent listenerEvent) =>
            await storageBroker.UpdateListenerEventAsync(listenerEvent);
    }
}
