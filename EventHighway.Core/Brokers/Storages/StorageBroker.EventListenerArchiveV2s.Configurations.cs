// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventListenerArchiveV2s(EntityTypeBuilder<EventListenerArchiveV2> model)
        {
            model.ToTable("EventListenerArchiveV2s");
            model.HasKey(eventListenerArchiveV2 => eventListenerArchiveV2.Id);
        }
    }
}
