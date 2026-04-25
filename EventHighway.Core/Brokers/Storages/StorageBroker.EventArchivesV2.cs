// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventArchiveV2> EventArchivesV2 { get; set; }

        public async ValueTask<EventArchiveV2> InsertEventArchiveV2Async(EventArchiveV2 eventArchive) =>
            await InsertAsync(eventArchive);

        public async ValueTask<IQueryable<EventArchiveV2>> SelectAllEventArchivesV2Async() =>
            SelectAll<EventArchiveV2>();

        public async ValueTask<EventArchiveV2> SelectEventArchiveByIdV2Async(Guid eventArchiveId) =>
            await SelectAsync<EventArchiveV2>(eventArchiveId);

        public async ValueTask<EventArchiveV2> DeleteEventArchiveV2Async(EventArchiveV2 eventArchive) =>
            await DeleteAsync(eventArchive);
    }
}
