// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventV1Archives(EntityTypeBuilder<EventV1Archive> model)
        {
            model.HasKey(eventV1Archive =>
                eventV1Archive.Id);
        }
    }
}
