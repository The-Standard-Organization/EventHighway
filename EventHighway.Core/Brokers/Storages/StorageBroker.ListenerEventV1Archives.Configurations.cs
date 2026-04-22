// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventV1Archives(EntityTypeBuilder<ListenerEventV1Archive> model)
        {
            model.HasKey(listenerEventV1Archive =>
                listenerEventV1Archive.Id);
        }
    }
}
