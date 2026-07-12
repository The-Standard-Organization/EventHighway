// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners;

namespace EventHighway.Core.Clients.EventListeners
{
    public interface IEventListenersClient
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventListener> RegisterEventListenerAsync(EventListener eventListener);

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<IQueryable<EventListener>> GetAllEventListenersAsync();
    }
}
