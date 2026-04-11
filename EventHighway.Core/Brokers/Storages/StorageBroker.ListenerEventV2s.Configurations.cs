// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
            model
                .ToTable("ListenerEventV2s");

            model
                .Property(listenerEventV2 => listenerEventV2.Id)
                .IsRequired();

            model.HasOne(listenerEventV2 => listenerEventV2.Event)
                .WithMany(eventV2 => eventV2.ListenerEvents)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasOne(listenerEventV2 => listenerEventV2.EventAddress)
                .WithMany(eventAddressV2 => eventAddressV2.ListenerEvents)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasOne(listenerEventV2 => listenerEventV2.EventListener)
                .WithMany(eventListenerV2 => eventListenerV2.ListenerEvents)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventListenerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
