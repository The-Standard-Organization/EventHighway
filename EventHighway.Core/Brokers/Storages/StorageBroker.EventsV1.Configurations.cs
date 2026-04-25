// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventsV1(EntityTypeBuilder<EventV1> model)
        {
            model.ToTable("EventV1s");
            model.HasKey(@event => @event.Id);

            model.HasOne(eventV1 => eventV1.EventAddress)
                .WithMany(eventAddressV1 => eventAddressV1.Events)
                .HasForeignKey(eventV1 => eventV1.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
