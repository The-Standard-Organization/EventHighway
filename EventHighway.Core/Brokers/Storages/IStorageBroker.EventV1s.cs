// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventV1> InsertEventV1Async(EventV1 eventV1);
        ValueTask<IQueryable<EventV1>> SelectAllEventV1sAsync();
        ValueTask<IQueryable<EventV1>> SelectAllEventV1sWithListenerEventV1sAsync();
        ValueTask<EventV1> SelectEventV1ByIdAsync(Guid eventV1Id);
        ValueTask<EventV1> UpdateEventV1Async(EventV1 eventV1);
        ValueTask<EventV1> DeleteEventV1Async(EventV1 eventV1);
    }
}
