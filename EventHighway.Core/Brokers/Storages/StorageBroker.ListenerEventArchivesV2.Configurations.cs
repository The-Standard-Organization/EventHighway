// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventV2Archives(EntityTypeBuilder<ListenerEventArchiveV2> model)
        {
            model
                .ToTable("ListenerEventArchiveV2s");

            model.HasKey(listenerEventV2Archive =>
                listenerEventV2Archive.Id);
        }
    }
}
