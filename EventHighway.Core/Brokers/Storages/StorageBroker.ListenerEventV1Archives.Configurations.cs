// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void ConfigureListenerEventV1Archives(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ListenerEventV1Archive>()
                .HasKey(listenerEventV1Archive =>
                    listenerEventV1Archive.Id);
        }
    }
}
