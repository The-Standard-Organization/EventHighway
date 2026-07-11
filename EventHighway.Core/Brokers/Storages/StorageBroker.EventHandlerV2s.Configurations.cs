// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventHandlerV2s(
            EntityTypeBuilder<EventHandlerV2> model)
        {
            model.ToTable("EventHandlerV2s");
            model.HasKey(eventHandlerV2 => eventHandlerV2.Id);

            model.Property(eventHandlerV2 => eventHandlerV2.Name)
                .IsRequired()
                .HasMaxLength(450);

            model.HasIndex(eventHandlerV2 => eventHandlerV2.Name)
                .IsUnique();
        }
    }
}
