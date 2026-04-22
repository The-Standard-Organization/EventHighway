// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventArchiveV2s(EntityTypeBuilder<EventArchiveV2> model)
        {
            model
                .ToTable("EventArchiveV2s");

            model.HasKey(eventV2Archive =>
                eventV2Archive.Id);
        }
    }
}
