// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventArchiveV2> EventArchiveV2s { get; set; }

        public async ValueTask<EventArchiveV2> InsertEventArchiveV2Async(EventArchiveV2 eventV2Archive) =>
            await InsertAsync(eventV2Archive);
    }
}
