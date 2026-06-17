// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventAddressV2s(EntityTypeBuilder<EventAddressV2> model)
        {
            model.ToTable("EventAddressV2s");
            model.HasKey(eventAddressV2 => eventAddressV2.Id);
        }
    }
}
