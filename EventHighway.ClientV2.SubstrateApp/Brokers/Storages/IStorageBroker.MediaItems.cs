// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<MediaItem> InsertMediaItemAsync(MediaItem mediaItem);
        ValueTask<IQueryable<MediaItem>> SelectAllMediaItemsAsync();
        ValueTask<MediaItem> SelectMediaItemByIdAsync(Guid mediaItemId);
        ValueTask<MediaItem> UpdateMediaItemAsync(MediaItem mediaItem);
        ValueTask<MediaItem> DeleteMediaItemAsync(MediaItem mediaItem);
    }
}
