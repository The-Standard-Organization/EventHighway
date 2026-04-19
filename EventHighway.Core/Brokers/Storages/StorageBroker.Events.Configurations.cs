// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEvents(EntityTypeBuilder<Event> model)
        {
            model.HasOne(@event => @event.EventAddress)
                .WithMany(eventAddress => eventAddress.Events)
                .HasForeignKey(@event => @event.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
