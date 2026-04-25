// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventArchivesV1(EntityTypeBuilder<EventArchiveV1> model)
        {
            model.ToTable("EventArchivesV1");
            model.HasKey(eventArchive => eventArchive.Id);
        }
    }
}
