// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V1
{
    internal interface IEventListenerV1OrchestrationService
    {
        ValueTask<EventListenerV1> AddEventListenerAsync(EventListenerV1 eventListener);
        ValueTask<IQueryable<EventListenerV1>> RetrieveEventListenersByEventAddressIdAsync(Guid eventAddressId);
        ValueTask<EventListenerV1> RemoveEventListenerByIdAsync(Guid eventListenerId);
        ValueTask<ListenerEventV1> AddListenerEventAsync(ListenerEventV1 listenerEventV1);
        ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync();
        ValueTask<ListenerEventV1> ModifyListenerEventAsync(ListenerEventV1 listenerEventV1);
        ValueTask<ListenerEventV1> RemoveListenerEventByIdAsync(Guid listenerEventV1Id);
    }
}
