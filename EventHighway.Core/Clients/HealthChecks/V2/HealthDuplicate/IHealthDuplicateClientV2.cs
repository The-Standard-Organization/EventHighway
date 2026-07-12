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
    /// Defines the contract for the V2 health duplicate client, providing the duplicate-detection
    /// summary for a period window.
    /// </summary>
    public interface IHealthDuplicateClientV2
    {
        /// <summary>
        /// Retrieves the duplicate-detection summary for the requested period and window.
        /// </summary>
        /// <param name="period">The period granularity to aggregate over.</param>
        /// <param name="windowStart">The inclusive UTC start of the window, or
        /// <see cref="DateTimeOffset.MinValue"/> for the current period.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{DuplicateDetectionSummaryV2}"/> representing the
        /// asynchronous operation that returns the duplicate-detection summary.</returns>
        /// <exception cref="HealthDuplicateClientV2ValidationException">Thrown when validation errors
        /// occur during retrieval.</exception>
        /// <exception cref="HealthDuplicateClientV2DependencyException">Thrown when dependency or
        /// service errors occur during retrieval.</exception>
        /// <exception cref="HealthDuplicateClientV2ServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<DuplicateDetectionSummaryV2> RetrieveDuplicateDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default);
    }
}
