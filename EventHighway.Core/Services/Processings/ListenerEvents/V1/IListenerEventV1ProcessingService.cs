// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V1
{
    internal interface IListenerEventV1ProcessingService
    {
        ValueTask<ListenerEventV1> AddListenerEventV1Async(ListenerEventV1 listenerEventV1);
        ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventV1sAsync();
        ValueTask<ListenerEventV1> ModifyListenerEventV1Async(ListenerEventV1 listenerEventV1);
        ValueTask<ListenerEventV1> RemoveListenerEventByIdAsync(Guid listenerEventV1Id);
    }
}
