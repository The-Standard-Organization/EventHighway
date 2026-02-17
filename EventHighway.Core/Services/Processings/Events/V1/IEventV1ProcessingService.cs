// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Processings.Events.V1
{
    internal interface IEventV1ProcessingService
    {
        ValueTask<EventV1> AddEventV1Async(EventV1 eventV1);
        ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventV1sAsync();
        ValueTask<IQueryable<EventV1>> RetrieveAllDeadEventV1sWithListenersAsync();
        ValueTask<EventV1> MarkEventV1AsImmediateAsync(EventV1 eventV1);
        ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id);
    }
}
