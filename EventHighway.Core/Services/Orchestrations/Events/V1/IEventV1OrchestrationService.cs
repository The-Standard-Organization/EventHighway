// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal interface IEventV1OrchestrationService
    {
        ValueTask<EventV1> SubmitEventAsync(EventV1 @event);
        ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventsAsync();
        ValueTask<EventV1> MarkEventAsImmediateAsync(EventV1 @event);
        ValueTask<EventV1> RemoveEventByIdAsync(Guid @eventId);
        ValueTask<EventCallV1> RunEventCallAsync(EventCallV1 eventCall);
        ValueTask<EventCallV1> RunEventCallAsyncV1(EventCallV1 eventCall);
    }
}
