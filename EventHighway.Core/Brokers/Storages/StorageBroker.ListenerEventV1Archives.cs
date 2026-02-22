// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventV1Archive> ListenerEventV1Archives { get; set; }

        public async ValueTask<ListenerEventV1Archive> InsertListenerEventV1ArchiveAsync(
            ListenerEventV1Archive listenerEventV1Archive)
        {
            return await InsertAsync(listenerEventV1Archive);
        }
    }
}
