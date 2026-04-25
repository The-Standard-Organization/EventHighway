// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal interface IEventV1CoordinationService
    {
        ValueTask<EventV1> SubmitEventAsync(EventV1 @event);
        ValueTask<EventV1> SubmitEventV1Async(EventV1 @event);
        ValueTask FireScheduledPendingEventsAsync();
        ValueTask<EventV1> RemoveEventByIdAsync(Guid eventId);
    }
}
