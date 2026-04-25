// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    public interface IEventV1OrchestrationServiceV1
    {
        ValueTask<IQueryable<EventV1>> RetrieveAllDeadEventsWithListenersAsync();
        ValueTask RemoveEventAndListenerEventsAsync(EventV1 @event);
    }
}
