// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaItems
{
    public partial interface IMediaItemService
    {
        ValueTask<MediaItem> AddMediaItemAsync(MediaItem mediaItem);
        ValueTask<IQueryable<MediaItem>> RetrieveAllMediaItemsAsync();
        ValueTask<MediaItem> RetrieveMediaItemByIdAsync(Guid mediaItemId);
        ValueTask<MediaItem> ModifyMediaItemAsync(MediaItem mediaItem);
        ValueTask<MediaItem> RemoveMediaItemByIdAsync(Guid mediaItemId);
    }
}
