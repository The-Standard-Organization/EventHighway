// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Storages
{
    // Every call goes through the shared gate: this context is a singleton, and in a web host two
    // requests can reach it at once. In practice a media item is only ever written from inside a
    // substrate dispatch, which already holds the gate — so these mostly pass straight through, and
    // the gate is here for the day something else touches the catalogue.
    internal partial class StorageBroker
    {
        public DbSet<MediaItem> MediaItems => Set<MediaItem>();

        public virtual ValueTask<MediaItem> InsertMediaItemAsync(MediaItem mediaItem) =>
            this.databaseGate.ExecuteAsync(() => InsertAsync(mediaItem));

        public virtual ValueTask<IQueryable<MediaItem>> SelectAllMediaItemsAsync() =>
            this.databaseGate.ExecuteAsync(() => SelectAllAsync<MediaItem>());

        public virtual ValueTask<MediaItem> SelectMediaItemByIdAsync(Guid mediaItemId) =>
            this.databaseGate.ExecuteAsync(() => SelectAsync<MediaItem>(mediaItemId));

        public virtual ValueTask<MediaItem> UpdateMediaItemAsync(MediaItem mediaItem) =>
            this.databaseGate.ExecuteAsync(() => UpdateAsync(mediaItem));

        public virtual ValueTask<MediaItem> DeleteMediaItemAsync(MediaItem mediaItem) =>
            this.databaseGate.ExecuteAsync(() => DeleteAsync(mediaItem));
    }
}
