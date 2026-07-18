// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.Events.V2
{
    internal interface IEventV2OrchestrationService
    {
        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> StampContentHashAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventV2>> RetrieveScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> MarkEventV2AsImmediateAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask<int> TryClaimScheduledEventV2Async(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<bool> IsLoopDetectedAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);
    }
}
