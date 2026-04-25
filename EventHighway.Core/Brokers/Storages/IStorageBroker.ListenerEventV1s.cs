// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<ListenerEventV1> InsertListenerEventV1Async(ListenerEventV1 listenerEvent);
        ValueTask<IQueryable<ListenerEventV1>> SelectAllListenerEventsV1Async();
        ValueTask<ListenerEventV1> SelectListenerEventByIdV1Async(Guid listenerEventId);
        ValueTask<ListenerEventV1> UpdateListenerEventV1Async(ListenerEventV1 listenerEvent);
        ValueTask<ListenerEventV1> DeleteListenerEventV1Async(ListenerEventV1 listenerEvent);
    }
}
