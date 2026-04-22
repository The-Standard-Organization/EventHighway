// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventListenerV2> InsertEventListenerV2Async(EventListenerV2 eventListenerV2);
        ValueTask<IQueryable<EventListenerV2>> SelectAllEventListenerV2sAsync();
        ValueTask<EventListenerV2> SelectEventListenerV2ByIdAsync(Guid eventListenerV2Id);
        ValueTask<EventListenerV2> DeleteEventListenerV2Async(EventListenerV2 eventListenerV2);
    }
}
