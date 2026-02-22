// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventV1Archive> EventV1Archives { get; set; }

        public async ValueTask<EventV1Archive> InsertEventV1ArchiveAsync(EventV1Archive eventV1Archive) =>
            await InsertAsync(eventV1Archive);
    }
}
