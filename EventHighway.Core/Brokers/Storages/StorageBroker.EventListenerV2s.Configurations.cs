// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventListenerV2s(EntityTypeBuilder<EventListenerV2> model)
        {
            model
                .ToTable("EventListenerV2s");

            model.HasKey(eventListenerV2 =>
                eventListenerV2.Id);

            model.HasOne(eventListenerV2 => eventListenerV2.EventAddress)
                .WithMany(eventAddressV2 => eventAddressV2.EventListeners)
                .HasForeignKey(eventListenerV2 => eventListenerV2.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
