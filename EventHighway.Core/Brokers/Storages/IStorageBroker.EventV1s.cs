// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventV1> InsertEventV1Async(EventV1 @event);
        ValueTask<IQueryable<EventV1>> SelectAllEventsV1Async();
        ValueTask<IQueryable<EventV1>> SelectAllEventsWithListenerEventsV1Async();
        ValueTask<EventV1> SelectEventByIdV1Async(Guid eventId);
        ValueTask<EventV1> UpdateEventV1Async(EventV1 @event);
        ValueTask<EventV1> DeleteEventV1Async(EventV1 @event);
    }
}
