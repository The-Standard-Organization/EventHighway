// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
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

            model.Property(eventArchiveV2 => eventArchiveV2.ContentHash)
                .IsRequired(true)
                .HasMaxLength(450);

            model.HasIndex(eventArchiveV2 => new
            {
                eventArchiveV2.EventAddressV2Id,
                eventArchiveV2.ContentHash
            })
            .HasDatabaseName("IX_EventArchiveV2s_ContentHash");

            model.HasIndex(eventArchiveV2 => eventArchiveV2.Status)
                .HasDatabaseName("IX_EventArchiveV2s_Status");

            model.HasIndex(eventArchiveV2 => eventArchiveV2.ArchivedDate)
                .HasDatabaseName("IX_EventArchiveV2s_ArchivedDate");

            model.HasIndex(eventArchiveV2 => new
            {
                eventArchiveV2.EventAddressV2Id,
                eventArchiveV2.ArchivedDate
            })
            .IncludeProperties(eventArchiveV2 => eventArchiveV2.Status)
            .HasDatabaseName("IX_EventArchiveV2s_AddressArchivedDate");

            model.HasMany<ListenerEventArchiveV2>(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                .WithOne()
                .HasForeignKey(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                .OnDelete(DeleteBehavior.Cascade);

            model.HasOne<EventAddressV2>(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                .WithMany(eventAddressV2 => eventAddressV2.EventArchiveV2s)
                .HasForeignKey(eventArchiveV2 => eventArchiveV2.EventAddressV2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
