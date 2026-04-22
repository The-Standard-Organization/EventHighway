// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventArchiveV2> ListenerEventArchiveV2s { get; set; }

        public async ValueTask<ListenerEventArchiveV2> InsertListenerEventV2ArchiveAsync(
            ListenerEventArchiveV2 listenerEventV2Archive)
        {
            return await InsertAsync(listenerEventV2Archive);
        }
    }
}
