// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Core.Services.Processings.EventAddresses.V2
{
    internal partial class EventAddressV2ProcessingService : IEventAddressV2ProcessingService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressV2ProcessingService(
            IEventAddressV2Service eventAddressV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();
        });

        public ValueTask<EventAddressV2> RetrieveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventAddressV2Id(eventAddressV2Id);

            return await this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                eventAddressV2Id,
                cancellationToken);
        });

        public ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateOnRemoveEventAddressV2ById(eventAddressV2Id);

            return await this.eventAddressV2Service.RemoveEventAddressV2ByIdAsync(
                eventAddressV2Id,
                cancellationToken);
        });

        public ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateOnRegisterEventAddressV2(eventAddressV2);

            return await this.eventAddressV2Service.AddEventAddressV2Async(
                eventAddressV2,
                cancellationToken);
        });

        public ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveOrRegisterEventAddressV2(eventAddressV2);

            IQueryable<EventAddressV2> allEventAddressV2s =
                await this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();

            EventAddressV2 maybeEventAddressV2 =
                allEventAddressV2s.FirstOrDefault(address => address.Id == eventAddressV2.Id);

            if (maybeEventAddressV2 is not null)
                return maybeEventAddressV2;

            return await this.eventAddressV2Service.AddEventAddressV2Async(
                eventAddressV2,
                cancellationToken);
        });
    }
}
