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
    /// Defines the contract for the V2 health retry client, providing the retry-health
    /// distribution across errored listener events for a period window.
    /// </summary>
    public interface IHealthRetryClientV2
    {
        /// <summary>
        /// Retrieves the retry-health distribution across errored listener events for the
        /// requested period and window.
        /// </summary>
        /// <param name="period">The period granularity to aggregate over.</param>
        /// <param name="windowStart">The inclusive UTC start of the window.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{RetryHealthSummaryV2}"/> representing the asynchronous
        /// operation that returns the retry-health summary.</returns>
        /// <exception cref="HealthRetryClientV2ValidationException">Thrown when validation errors
        /// occur during retrieval.</exception>
        /// <exception cref="HealthRetryClientV2DependencyException">Thrown when dependency or
        /// service errors occur during retrieval.</exception>
        /// <exception cref="HealthRetryClientV2ServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default);
    }
}
