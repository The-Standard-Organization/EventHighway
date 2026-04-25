// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Services.Foundations.ListenerEvents.V1
{
    internal interface IListenerEventV1Service
    {
        ValueTask<ListenerEventV1> AddListenerEventAsync(ListenerEventV1 listenerEvent);
        ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync();
        ValueTask<ListenerEventV1> ModifyListenerEventAsync(ListenerEventV1 listenerEvent);
        ValueTask<ListenerEventV1> RemoveListenerEventByIdAsync(Guid listenerEventId);
    }
}
