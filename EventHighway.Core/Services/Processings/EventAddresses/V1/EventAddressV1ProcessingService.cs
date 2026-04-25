// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Services.Processings.EventAddresses.V1
{
    internal partial class EventAddressV1ProcessingService : IEventAddressV1ProcessingService
    {
        private readonly IEventAddressV1Service eventAddressV1Service;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressV1ProcessingService(
            IEventAddressV1Service eventAddressV1Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV1Service = eventAddressV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventAddressV1> RetrieveEventAddressByIdAsync(Guid eventAddressId) =>
        TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressId);

            return await this.eventAddressV1Service.RetrieveEventAddressByIdAsync(
                eventAddressId);
        });
    }
}
