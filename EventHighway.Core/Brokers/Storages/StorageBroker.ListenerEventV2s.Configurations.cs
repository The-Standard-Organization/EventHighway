// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventV2s(EntityTypeBuilder<ListenerEventV2> model)
        {
            model.ToTable("ListenerEventV2s");
            model.HasKey(listenerEventV2 => listenerEventV2.Id);

            model
                .Property(listenerEventV2 => listenerEventV2.Id)
                .IsRequired();

            model.HasIndex(listenerEventV2 => new
            {
                listenerEventV2.Status,
                listenerEventV2.RemainingRetryAttempts
            })
            .HasDatabaseName("IX_ListenerEventV2s_RetryClassification");

            model.HasIndex(listenerEventV2 => listenerEventV2.CreatedDate)
                .IncludeProperties(listenerEventV2 => listenerEventV2.Status)
                .HasDatabaseName("IX_ListenerEventV2s_CreatedDate");

            model.HasIndex(listenerEventV2 => new
            {
                listenerEventV2.EventAddressV2Id,
                listenerEventV2.CreatedDate
            })
            .IncludeProperties(listenerEventV2 => new
            {
                listenerEventV2.Status,
                listenerEventV2.RemainingRetryAttempts
            })
            .HasDatabaseName("IX_ListenerEventV2s_AddressCreatedDate");

            model.HasIndex(listenerEventV2 => listenerEventV2.EventV2Id)
                .IncludeProperties(listenerEventV2 => new
                {
                    listenerEventV2.Status,
                    listenerEventV2.RemainingRetryAttempts,
                    listenerEventV2.DispatchedDate
                })
                .HasDatabaseName("IX_ListenerEventV2s_ToBeArchived");

            model.HasOne(listenerEventV2 => listenerEventV2.EventV2)
                .WithMany(eventV2 => eventV2.ListenerEventV2s)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventV2Id)
                .OnDelete(DeleteBehavior.Cascade);

            model.HasOne(listenerEventV2 => listenerEventV2.EventAddressV2)
                .WithMany(eventAddressV2 => eventAddressV2.ListenerEventV2s)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventAddressV2Id)
                .OnDelete(DeleteBehavior.Restrict);

            model.HasOne(listenerEventV2 => listenerEventV2.EventListenerV2)
                .WithMany(eventListenerV2 => eventListenerV2.ListenerEventV2s)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventListenerV2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
