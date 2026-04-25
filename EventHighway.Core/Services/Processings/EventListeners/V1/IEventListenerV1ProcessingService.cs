// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Services.Processings.EventListeners.V1
{
    internal interface IEventListenerV1ProcessingService
    {
        ValueTask<EventListenerV1> AddEventListenerAsync(EventListenerV1 eventListenerV1);
        ValueTask<IQueryable<EventListenerV1>> RetrieveEventListenersByEventAddressIdAsync(Guid eventAddressId);
        ValueTask<EventListenerV1> RemoveEventListenerByIdAsync(Guid eventListenerV1Id);
    }
}
