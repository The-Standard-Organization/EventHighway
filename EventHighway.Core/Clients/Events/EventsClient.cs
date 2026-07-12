// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events;
using EventHighway.Core.Services.Coordinations.Events;

namespace EventHighway.Core.Clients.Events
{
    public class EventsClient : IEventsClient
    {
        private readonly IEventCoordinationService eventCoordinationService;

        public EventsClient(IEventCoordinationService eventCoordinationService) =>
            this.eventCoordinationService = eventCoordinationService;

        [Obsolete("This function is deprecated use the latest version instead.")]
        public async ValueTask<Event> SubmitEventAsync(Event @event) =>
            await this.eventCoordinationService.SubmitEventAsync(@event);
    }
}
