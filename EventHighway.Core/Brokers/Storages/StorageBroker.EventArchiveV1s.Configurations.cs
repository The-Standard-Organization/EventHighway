// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventArchiveV1s(EntityTypeBuilder<EventArchiveV1> model)
        {
            model.ToTable("EventArchiveV1s");
            model.HasKey(eventArchiveV1 => eventArchiveV1.Id);
        }
    }
}
