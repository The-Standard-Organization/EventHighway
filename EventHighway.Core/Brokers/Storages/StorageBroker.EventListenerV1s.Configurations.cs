// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventListenerV1s(EntityTypeBuilder<EventListenerV1> model)
        {
            model.ToTable("EventListenerV1s");
            model.HasKey(eventListenerV1 => eventListenerV1.Id);

            model.HasOne(eventListenerV1 => eventListenerV1.EventAddressV1s)
                .WithMany(eventAddressV1 => eventAddressV1.EventListenerV1s)
                .HasForeignKey(eventListenerV1 => eventListenerV1.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
