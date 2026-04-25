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
        private static void ConfigureEventAddressesV2(EntityTypeBuilder<EventAddressV2> model)
        {
            model.ToTable("EventAddressesV2");
            model.HasKey(eventAddress => eventAddress.Id);
        }
    }
}
