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
        ValueTask<EventV1> AddEventAsync(EventV1 eventV1);
        ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventsAsync();
        ValueTask<IQueryable<EventV1>> RetrieveAllDeadEventsWithListenersAsync();
        ValueTask<EventV1> MarkEventAsImmediateAsync(EventV1 eventV1);
        ValueTask<EventV1> RemoveEventByIdAsync(Guid eventV1Id);
    }
}
