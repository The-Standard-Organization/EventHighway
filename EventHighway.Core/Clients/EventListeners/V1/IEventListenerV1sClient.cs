// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;

namespace EventHighway.Core.Clients.EventListeners.V1
{
    public interface IEventListenerV1sClient
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventListenerV1> RegisterEventListenerV1Async(EventListenerV1 eventListenerV1);

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventListenerV1> RemoveEventListenerV1ByIdAsync(Guid eventListenerV1Id);
    }
}
