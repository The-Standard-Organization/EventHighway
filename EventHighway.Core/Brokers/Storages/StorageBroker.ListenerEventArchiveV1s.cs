// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventArchiveV1> ListenerEventArchiveV1s { get; set; }

        public async ValueTask<ListenerEventArchiveV1> InsertListenerEventArchiveV1Async(
            ListenerEventArchiveV1 listenerEventArchiveV1) =>
            await InsertAsync(listenerEventArchiveV1);
    }
}
