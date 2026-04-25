// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventArchives.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventArchivesV2(EntityTypeBuilder<EventArchiveV2> model)
        {
            model.ToTable("EventArchivesV2");
            model.HasKey(eventArchive => eventArchive.Id);
        }
    }
}
