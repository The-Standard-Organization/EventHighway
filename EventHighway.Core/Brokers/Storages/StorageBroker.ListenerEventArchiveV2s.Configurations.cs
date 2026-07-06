// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventArchiveV2s(EntityTypeBuilder<ListenerEventArchiveV2> model)
        {
            model.ToTable("ListenerEventArchiveV2s");
            model.HasKey(listenerEventArchiveV2 => listenerEventArchiveV2.Id);

            model.Property(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                .IsRequired();

            model.HasIndex(listenerEventArchiveV2 => new
            {
                listenerEventArchiveV2.Status,
                listenerEventArchiveV2.RemainingRetryAttempts
            })
            .HasDatabaseName("IX_ListenerEventArchiveV2s_RetryClassification");

            model.HasIndex(listenerEventArchiveV2 => listenerEventArchiveV2.Status)
                .HasDatabaseName("IX_ListenerEventArchiveV2s_Status");

            model.HasIndex(listenerEventArchiveV2 => listenerEventArchiveV2.ArchivedDate)
                .IncludeProperties(listenerEventArchiveV2 => listenerEventArchiveV2.Status)
                .HasDatabaseName("IX_ListenerEventArchiveV2s_ArchivedDate");

            model.HasIndex(listenerEventArchiveV2 => new
            {
                listenerEventArchiveV2.EventAddressV2Id,
                listenerEventArchiveV2.ArchivedDate
            })
            .IncludeProperties(listenerEventArchiveV2 => listenerEventArchiveV2.Status)
            .HasDatabaseName("IX_ListenerEventArchiveV2s_AddressArchivedDate");

            model.HasOne<EventListenerV2>(listenerEventArchiveV2 => listenerEventArchiveV2.EventListenerV2)
                .WithMany(eventListenerV2 => eventListenerV2.ListenerEventArchiveV2s)
                .HasForeignKey(listenerEventArchiveV2 => listenerEventArchiveV2.EventListenerV2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
