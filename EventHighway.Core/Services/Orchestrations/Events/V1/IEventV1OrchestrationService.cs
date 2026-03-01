// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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
        ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1);
        ValueTask<IQueryable<EventV1>> RetrieveScheduledPendingEventV1sAsync();
        ValueTask<EventV1> MarkEventV1AsImmediateAsync(EventV1 eventV1);
        ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id);
        ValueTask<EventCallV1> RunEventCallV1Async(EventCallV1 eventCallV1);
        ValueTask<EventCallV1> RunEventCallV1AsyncV1(EventCallV1 eventCallV1);
    }
}
