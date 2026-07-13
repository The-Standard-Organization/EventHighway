// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners;
using EventHighway.Core.Services.Foundations.EventListeners;

namespace EventHighway.Core.Clients.EventListeners
{
    public class EventListenersClient : IEventListenersClient
    {
        private readonly IEventListenerService eventListenerService;

        public EventListenersClient(IEventListenerService eventListenerService) =>
            this.eventListenerService = eventListenerService;

        [Obsolete("This function is deprecated use the latest version instead.")]
        public async ValueTask<IQueryable<EventListener>> GetAllEventListenersAsync() =>
            await this.eventListenerService.RetrieveAllEventListenersAsync();

        [Obsolete("This function is deprecated use the latest version instead.")]
        public async ValueTask<EventListener> RegisterEventListenerAsync(EventListener eventListener) =>
            await this.eventListenerService.AddEventListenerAsync(eventListener);
    }
}
