// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventArchiveV1s(EntityTypeBuilder<ListenerEventArchiveV1> model)
        {
            model.HasKey(listenerEventArchiveV1 =>
                listenerEventArchiveV1.Id);
        }
    }
}
