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
        private static void ConfigureEventV2s(EntityTypeBuilder<EventV2> model)
        {
            model
                .ToTable("EventV2s");

            model.HasKey(eventV2 =>
                eventV2.Id);

            model.HasOne(eventV2 => eventV2.EventAddress)
                .WithMany(eventAddressV2 => eventAddressV2.Events)
                .HasForeignKey(eventV2 => eventV2.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
