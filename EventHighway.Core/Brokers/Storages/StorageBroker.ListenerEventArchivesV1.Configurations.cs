// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureListenerEventArchivesV1(EntityTypeBuilder<ListenerEventArchiveV1> model)
        {
            model.ToTable("ListenerEventArchivesV1");
            model.HasKey(listenerEventArchive => listenerEventArchive.Id);
        }
    }
}
