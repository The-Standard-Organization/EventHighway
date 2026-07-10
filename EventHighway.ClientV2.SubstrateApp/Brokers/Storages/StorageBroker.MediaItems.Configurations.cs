// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void AddMediaItemConfigurations(EntityTypeBuilder<MediaItem> model)
        {
            model.ToTable("MediaItems");
            model.HasKey(mediaItem => mediaItem.Id);
            model.Property(mediaItem => mediaItem.Id).IsRequired();
        }
    }
}
