// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEvent> ListenerEvents { get; set; }

        public async ValueTask<ListenerEvent> InsertListenerEventAsync(ListenerEvent listenerEvent) =>
            await this.InsertAsync(listenerEvent);

        public async ValueTask<ListenerEvent> UpdateListenerEventAsync(ListenerEvent listenerEvent) =>
            await this.UpdateAsync(listenerEvent);
    }
}
