// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventArchiveV1> EventArchivesV1 { get; set; }

        public async ValueTask<EventArchiveV1> InsertEventArchiveV1Async(EventArchiveV1 eventArchive) =>
            await InsertAsync(eventArchive);

        public async ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchivesV1Async() =>
            SelectAll<EventArchiveV1>();

        public async ValueTask<EventArchiveV1> SelectEventArchiveByIdV1Async(Guid eventArchiveId) =>
            await SelectAsync<EventArchiveV1>(eventArchiveId);

        public async ValueTask<EventArchiveV1> DeleteEventArchiveV1Async(EventArchiveV1 eventArchive) =>
            await DeleteAsync(eventArchive);
    }
}
