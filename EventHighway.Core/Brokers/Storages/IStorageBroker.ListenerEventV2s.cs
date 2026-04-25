// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<ListenerEventV2> InsertListenerEventV2Async(ListenerEventV2 listenerEvent);
        ValueTask<IQueryable<ListenerEventV2>> SelectAllListenerEventsV2Async();
        ValueTask<ListenerEventV2> SelectListenerEventByIdV2Async(Guid listenerEventId);
        ValueTask<ListenerEventV2> UpdateListenerEventV2Async(ListenerEventV2 listenerEvent);
        ValueTask<ListenerEventV2> DeleteListenerEventV2Async(ListenerEventV2 listenerEvent);
    }
}
