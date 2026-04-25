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
        ValueTask<EventV2> InsertEventV2Async(EventV2 @event);
        ValueTask<IQueryable<EventV2>> SelectAllEventsV2Async();
        ValueTask<EventV2> SelectEventByIdV2Async(Guid eventId);
        ValueTask<EventV2> UpdateEventV2Async(EventV2 @event);
        ValueTask<EventV2> DeleteEventV2Async(EventV2 @event);
    }
}
