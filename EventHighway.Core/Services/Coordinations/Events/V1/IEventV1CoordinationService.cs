// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal interface IEventV1CoordinationService
    {
        ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1);
        ValueTask<EventV1> SubmitEventV1AsyncV1(EventV1 eventV1);
        ValueTask FireScheduledPendingEventV1sAsync();
        ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id);
    }
}
