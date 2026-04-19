// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureHandlerConfigurations(EntityTypeBuilder<HandlerConfiguration> model)
        {
            model
                .ToTable("HandlerConfigurations");

            model.HasKey(handlerConfiguration =>
                handlerConfiguration.Id);

            model.HasOne(handlerConfiguration => handlerConfiguration.EventListener)
                .WithMany(eventListenerV2 => eventListenerV2.HandlerConfigurations)
                .HasForeignKey(handlerConfiguration => handlerConfiguration.EventListenerId)
                .OnDelete(DeleteBehavior.NoAction);

            model.HasIndex(handlerConfiguration => new
            {
                handlerConfiguration.EventListenerId,
                handlerConfiguration.Name
            }).IsUnique();
        }
    }
}
