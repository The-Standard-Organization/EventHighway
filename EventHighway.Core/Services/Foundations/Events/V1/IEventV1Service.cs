// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Foundations.Events.V1
{
    internal partial interface IEventV1Service
    {
        ValueTask<EventV1> AddEventAsync(EventV1 eventV1);
        ValueTask<IQueryable<EventV1>> RetrieveAllEventsAsync();
        ValueTask<IQueryable<EventV1>> RetrieveAllEventsWithListenerEventsAsync();
        ValueTask<EventV1> ModifyEventAsync(EventV1 eventV1);
        ValueTask<EventV1> RemoveEventByIdAsync(Guid eventV1Id);
    }
}
