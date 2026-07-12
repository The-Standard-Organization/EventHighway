// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Clients.Events.V1
{
    public interface IEventV1sClient
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1);

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventV1> SubmitEventV1AsyncV1(EventV1 eventV1);

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask FireScheduledPendingEventV1sAsync();

        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id);
    }
}
