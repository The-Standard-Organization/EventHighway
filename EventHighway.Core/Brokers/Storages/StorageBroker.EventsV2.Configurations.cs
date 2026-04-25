// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventsV2(EntityTypeBuilder<EventV2> model)
        {
            model.ToTable("EventsV2");
            model.HasKey(@event => @event.Id);

            model.HasOne(@event => @event.EventAddress)
                .WithMany(eventAddressV2 => eventAddressV2.Events)
                .HasForeignKey(@event => @event.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
