// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventListenerV1> InsertEventListenerV1Async(EventListenerV1 eventListener);
        ValueTask<IQueryable<EventListenerV1>> SelectAllEventListenersV1Async();
        ValueTask<EventListenerV1> SelectEventListenerByIdV1Async(Guid eventListenerId);
        ValueTask<EventListenerV1> DeleteEventListenerV1Async(EventListenerV1 eventListener);
    }
}
