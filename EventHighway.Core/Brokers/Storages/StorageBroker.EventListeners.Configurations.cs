using EventHighway.Core.Models.Services.Foundations.EventListeners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventListeners(EntityTypeBuilder<EventListener> model)
        {
            model.HasOne(eventListener => eventListener.EventAddress)
                .WithMany(eventAddress => eventAddress.EventListeners)
                .HasForeignKey(eventListener => eventListener.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
