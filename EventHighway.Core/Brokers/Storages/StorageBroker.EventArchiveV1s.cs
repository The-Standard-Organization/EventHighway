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
        public DbSet<EventArchiveV1> EventArchiveV1s { get; set; }

        public async ValueTask<EventArchiveV1> InsertEventArchiveV1Async(EventArchiveV1 eventArchiveV1) =>
            await InsertAsync(eventArchiveV1);

        public async ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchiveV1sAsync() =>
            SelectAll<EventArchiveV1>();

        public async ValueTask<EventArchiveV1> SelectEventArchiveV1ByIdAsync(Guid eventArchiveV1Id) =>
            await SelectAsync<EventArchiveV1>(eventArchiveV1Id);

        public async ValueTask<EventArchiveV1> DeleteEventArchiveV1Async(EventArchiveV1 eventArchiveV1V1) =>
            await DeleteAsync(eventArchiveV1V1);
    }
}
