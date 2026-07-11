// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;

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
            throw new NotImplementedException();

        public void RegisterEventHandlerV2(IEventHandler eventHandler) =>
            TryCatch(() =>
            {
                ValidateEventHandlerV2OnRegister(eventHandler);
                this.eventHandlerBroker.Register(eventHandler);
            });

        public IEnumerable<IEventHandler> RetrieveAllEventHandlerV2s() =>
            TryCatch(() => this.eventHandlerBroker.GetAll());
    }
}
