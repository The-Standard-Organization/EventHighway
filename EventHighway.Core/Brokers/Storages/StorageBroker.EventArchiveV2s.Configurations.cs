// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventArchiveV2s(EntityTypeBuilder<EventArchiveV2> model)
        {
            model.ToTable("EventArchiveV2s");
            model.HasKey(eventArchiveV2 => eventArchiveV2.Id);

            model.HasMany<EventListenerArchiveV2>(eventArchiveV2 => eventArchiveV2.EventListenerArchiveV2s)
                .WithOne()
                .HasForeignKey(eventListenerArchiveV2 => eventListenerArchiveV2.EventArchiveV2Id)
                .OnDelete(DeleteBehavior.Cascade);

            model.HasMany<ListenerEventArchiveV2>(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                .WithOne()
                .HasForeignKey(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
