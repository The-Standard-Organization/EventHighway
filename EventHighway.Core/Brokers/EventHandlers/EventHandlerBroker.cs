// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Brokers.EventHandlers
{
    internal class EventHandlerBroker : IEventHandlerBroker
    {
        private readonly ConcurrentDictionary<Guid, IEventHandler> eventHandlers =
            new ConcurrentDictionary<Guid, IEventHandler>();

        public void Register(IEventHandler eventHandler) =>
            this.eventHandlers[eventHandler.Id] = eventHandler;

        public void Remove(Guid eventHandlerId) =>
            this.eventHandlers.TryRemove(eventHandlerId, out _);

        public IEnumerable<IEventHandler> GetAll() =>
            this.eventHandlers.Values.ToArray();
    }
}
