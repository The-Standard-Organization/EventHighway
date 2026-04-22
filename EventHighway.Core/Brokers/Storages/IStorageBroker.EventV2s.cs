// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventV2> InsertEventV2Async(EventV2 eventV2);
        ValueTask<IQueryable<EventV2>> SelectAllEventV2sAsync();
        ValueTask<EventV2> SelectEventV2ByIdAsync(Guid eventV2Id);
        ValueTask<EventV2> UpdateEventV2Async(EventV2 eventV2);
        ValueTask<EventV2> DeleteEventV2Async(EventV2 eventV2);
    }
}
