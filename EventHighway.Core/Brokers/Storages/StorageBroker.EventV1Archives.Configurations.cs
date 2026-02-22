// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void ConfigureEventV1Archives(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventV1Archive>()
                .HasKey(eventV1Archive =>
                    eventV1Archive.Id);
        }
    }
}
