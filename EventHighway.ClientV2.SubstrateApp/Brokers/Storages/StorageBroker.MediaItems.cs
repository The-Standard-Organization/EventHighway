// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<MediaItem> MediaItems => Set<MediaItem>();

        public virtual async ValueTask<MediaItem> InsertMediaItemAsync(MediaItem mediaItem) =>
            await InsertAsync(mediaItem);

        public virtual async ValueTask<IQueryable<MediaItem>> SelectAllMediaItemsAsync() =>
            await SelectAllAsync<MediaItem>();

        public virtual async ValueTask<MediaItem> SelectMediaItemByIdAsync(Guid mediaItemId) =>
            await SelectAsync<MediaItem>(mediaItemId);

        public virtual async ValueTask<MediaItem> UpdateMediaItemAsync(MediaItem mediaItem) =>
            await UpdateAsync(mediaItem);

        public virtual async ValueTask<MediaItem> DeleteMediaItemAsync(MediaItem mediaItem) =>
            await DeleteAsync(mediaItem);
    }
}
