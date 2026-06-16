// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.EventListeners.V2;

namespace EventHighway.Core.Services.Processings.EventListeners.V2
{
    internal partial class EventListenerV2ProcessingService : IEventListenerV2ProcessingService
    {
        private readonly IEventListenerV2Service eventListenerV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventListenerV2ProcessingService(
            IEventListenerV2Service eventListenerV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventListenerV2Service = eventListenerV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventListenerV2>> RetrieveAllEventListenerV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.eventListenerV2Service.RetrieveAllEventListenerV2sAsync();
        });

        public ValueTask<EventListenerV2> AddEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV2IsNotNull(eventListenerV2);

            return await this.eventListenerV2Service.AddEventListenerV2Async(
                eventListenerV2,
                cancellationToken);
        });

        public ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default) => TryCatch(async () =>
        {
            ValidateEventAddressId(eventAddressId);

            IQueryable<EventListenerV2> eventListenerV2s =
                await this.eventListenerV2Service.RetrieveAllEventListenerV2sAsync();

            return eventListenerV2s.Where(eventListenerV2 =>
                eventListenerV2.EventAddressId == eventAddressId);
        });

        public ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventListenerV2Id(eventListenerV2Id);

            return await this.eventListenerV2Service.RemoveEventListenerV2ByIdAsync(
                eventListenerV2Id,
                cancellationToken);
        });

        public ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveOrRegisterEventListenerV2(eventListenerV2);

            IQueryable<EventListenerV2> allEventListenerV2s =
                await this.eventListenerV2Service.RetrieveAllEventListenerV2sAsync();

            EventListenerV2 maybeEventListenerV2 =
                allEventListenerV2s.FirstOrDefault(listener => listener.Id == eventListenerV2.Id);

            if (maybeEventListenerV2 is not null)
                return maybeEventListenerV2;

            return await this.eventListenerV2Service.AddEventListenerV2Async(
                eventListenerV2,
                cancellationToken);
        });
    }
}
