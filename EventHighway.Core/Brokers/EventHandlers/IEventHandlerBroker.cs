// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Brokers.EventHandlers
{
    internal interface IEventHandlerBroker
    {
        void Register(IEventHandler eventHandler);
        void Remove(Guid eventHandlerId);
        IEnumerable<IEventHandler> GetAll();
    }
}
