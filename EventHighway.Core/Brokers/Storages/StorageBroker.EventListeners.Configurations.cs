// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventListeners(EntityTypeBuilder<EventListener> model)
        {
            model.ToTable("EventListeners");
            model.HasKey(eventListener => eventListener.Id);

            model.HasOne(eventListener => eventListener.EventAddress)
                .WithMany(eventAddress => eventAddress.EventListeners)
                .HasForeignKey(eventListener => eventListener.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
