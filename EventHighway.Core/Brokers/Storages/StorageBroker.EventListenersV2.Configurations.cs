// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void ConfigureEventListenersV2(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventListenerV2>()
                .HasOne(eventListenerV2 => eventListenerV2.EventAddress)
                .WithMany(eventAddressV2 => eventAddressV2.EventListeners)
                .HasForeignKey(eventListenerV2 => eventListenerV2.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
