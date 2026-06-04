// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventAddressV1s(EntityTypeBuilder<EventAddressV1> model)
        {
            model.ToTable("EventAddressV1s");
            model.HasKey(eventAddressV1 => eventAddressV1.Id);
        }
    }
}
