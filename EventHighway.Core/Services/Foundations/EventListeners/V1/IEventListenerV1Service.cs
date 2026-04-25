// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Services.Foundations.EventListeners.V1
{
    internal interface IEventListenerV1Service
    {
        ValueTask<EventListenerV1> AddEventListenerAsync(EventListenerV1 eventListener);
        ValueTask<IQueryable<EventListenerV1>> RetrieveAllEventListenersAsync();
        ValueTask<EventListenerV1> RemoveEventListenerByIdAsync(Guid eventListenerId);
    }
}
