// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Services.Processings.EventAddresses.V1
{
    internal interface IEventAddressV1ProcessingService
    {
        ValueTask<EventAddressV1> RetrieveEventAddressByIdAsync(Guid eventAddressId);
    }
}
