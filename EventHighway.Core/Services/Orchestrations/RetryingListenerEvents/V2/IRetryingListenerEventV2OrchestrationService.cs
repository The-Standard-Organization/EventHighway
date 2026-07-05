// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2
{
    internal interface IRetryingListenerEventV2OrchestrationService
    {
        ValueTask<ListenerEventV2> RetryListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default);

        ValueTask RetryFailedListenerEventV2sAsync(
            CancellationToken cancellationToken = default);
    }
}
