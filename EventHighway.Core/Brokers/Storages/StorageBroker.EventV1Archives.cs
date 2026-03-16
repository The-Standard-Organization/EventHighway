// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
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

        public async ValueTask<IQueryable<EventV1Archive>> SelectAllEventV1ArchivesAsync() =>
            SelectAll<EventV1Archive>();

        public async ValueTask<EventV1Archive> SelectEventV1ArchiveByIdAsync(Guid eventV1ArchiveId) =>
            await SelectAsync<EventV1Archive>(eventV1ArchiveId);

        public async ValueTask<int> DeleteEventV1ArchivesAsync(DateTimeOffset? cutOffDate) =>
            await ExecuteDeleteAsync<EventV1Archive>(entity => entity.CreatedDate < cutOffDate);
    }
}
