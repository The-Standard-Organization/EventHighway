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
        ValueTask<EventV1> AddEventV1Async(EventV1 eventV1);
        ValueTask<IQueryable<EventV1>> RetrieveAllEventV1sAsync();
        ValueTask<IQueryable<EventV1>> RetrieveAllEventV1sWithListenerEventV1sAsync();
        ValueTask<EventV1> ModifyEventV1Async(EventV1 eventV1);
        ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id);
    }
}
