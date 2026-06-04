// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventV1s(EntityTypeBuilder<ListenerEventV1> model)
        {
            model.ToTable("ListenerEventV1s");
            model.HasKey(listenerEventV1 => listenerEventV1.Id);

            model.HasOne(listenerEventV1 => listenerEventV1.Event)
                .WithMany(eventV1 => eventV1.ListenerEventV1s)
                .HasForeignKey(listenerEventV1 => listenerEventV1.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasOne(listenerEventV1 => listenerEventV1.EventAddress)
                .WithMany(eventAddressV1 => eventAddressV1.ListenerEventV1s)
                .HasForeignKey(listenerEventV1 => listenerEventV1.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasOne(listenerEventV1 => listenerEventV1.EventListener)
                .WithMany(eventListenerV1 => eventListenerV1.ListenerEventV1s)
                .HasForeignKey(listenerEventV1 => listenerEventV1.EventListenerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
