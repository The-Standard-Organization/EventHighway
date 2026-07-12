// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Defines the contract for the V2 health traffic client, providing time-bucketed event
    /// and listener-event traffic for a period window.
    /// </summary>
    public interface IHealthTrafficClientV2
    {
        /// <summary>
        /// Retrieves a time-bucketed traffic snapshot for the requested period and window.
        /// </summary>
        /// <param name="period">The period granularity to aggregate over.</param>
        /// <param name="windowStart">The inclusive UTC start of the window, or
        /// <see cref="DateTimeOffset.MinValue"/> for the current period.</param>
        /// <param name="windowEnd">The exclusive UTC end of the window; required when
        /// <paramref name="period"/> is <see cref="TrafficPeriodV2.Custom"/>, derived from the
        /// period otherwise.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{TrafficSnapshotV2}"/> representing the asynchronous
        /// operation that returns the traffic snapshot.</returns>
        /// <exception cref="HealthTrafficClientV2ValidationException">Thrown when validation errors
        /// occur during retrieval.</exception>
        /// <exception cref="HealthTrafficClientV2DependencyException">Thrown when dependency or
        /// service errors occur during retrieval.</exception>
        /// <exception cref="HealthTrafficClientV2ServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default);
    }
}
