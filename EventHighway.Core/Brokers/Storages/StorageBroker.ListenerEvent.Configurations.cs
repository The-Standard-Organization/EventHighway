// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEvents(EntityTypeBuilder<ListenerEvent> model)
        {
            model.HasOne(listenerEvent => listenerEvent.Event)
                .WithMany(@event => @event.ListenerEvents)
                .HasForeignKey(listenerEvent => listenerEvent.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasOne(listenerEvent => listenerEvent.EventAddress)
                .WithMany(eventAddress => eventAddress.ListenerEvents)
                .HasForeignKey(listenerEvent => listenerEvent.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasOne(listenerEvent => listenerEvent.EventListener)
                .WithMany(eventListener => eventListener.ListenerEvents)
                .HasForeignKey(listenerEvent => listenerEvent.EventListenerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
