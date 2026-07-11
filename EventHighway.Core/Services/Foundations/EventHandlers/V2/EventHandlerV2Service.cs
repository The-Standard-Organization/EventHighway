// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service : IEventHandlerV2Service
    {
        private readonly IEventHandlerBroker eventHandlerBroker;
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventHandlerV2Service(
            IEventHandlerBroker eventHandlerBroker,
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHandlerBroker = eventHandlerBroker;
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEventHandler> AddEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventHandlerV2OnAdd(eventHandler);

            var eventHandlerV2 = new EventHandlerV2
            {
                Id = eventHandler.Id,
                Name = eventHandler.Name
            };

            await this.storageBroker.InsertEventHandlerV2Async(eventHandlerV2, cancellationToken);
            this.eventHandlerBroker.Register(eventHandler);

            return eventHandler;
        }));

        public void RegisterEventHandlerV2(IEventHandler eventHandler) =>
            TryCatch(() =>
            {
                ValidateEventHandlerV2OnRegister(eventHandler);
                this.eventHandlerBroker.Register(eventHandler);
            });

        public IEnumerable<IEventHandler> RetrieveAllEventHandlerV2s() =>
            TryCatch(() => this.eventHandlerBroker.GetAll());

        public ValueTask<IQueryable<IEventHandler>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
