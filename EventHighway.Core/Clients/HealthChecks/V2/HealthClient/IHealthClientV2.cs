// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Defines the container/exposer for the V2 health check sub-clients. This is a pure exposer
    /// with no retrieval method of its own — each dashboard panel is served by its own sub-client
    /// so it can be pulled and refreshed independently.
    /// </summary>
    public interface IHealthClientV2
    {
        /// <summary>
        /// Gets the sub-client for the whole-system RAG health check items.
        /// </summary>
        IHealthStatusClientV2 HealthStatusClientV2 { get; }

        /// <summary>
        /// Gets the sub-client for the time-bucketed traffic snapshot.
        /// </summary>
        IHealthTrafficClientV2 HealthTrafficClientV2 { get; }

        /// <summary>
        /// Gets the sub-client for the per-event-address usage roll-up.
        /// </summary>
        IHealthAddressClientV2 HealthAddressClientV2 { get; }

        /// <summary>
        /// Gets the sub-client for the loop-detection summary.
        /// </summary>
        IHealthLoopClientV2 HealthLoopClientV2 { get; }

        /// <summary>
        /// Gets the sub-client for the duplicate-detection summary.
        /// </summary>
        IHealthDuplicateClientV2 HealthDuplicateClientV2 { get; }

        /// <summary>
        /// Gets the sub-client for the retry-health summary.
        /// </summary>
        IHealthRetryClientV2 HealthRetryClientV2 { get; }

        /// <summary>
        /// Gets the sub-client for the per-participant usage roll-up.
        /// </summary>
        IHealthParticipantClientV2 HealthParticipantClientV2 { get; }
    }
}
