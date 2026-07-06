// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the whole-system infrastructure snapshot: totals for registered addresses,
    /// listeners, and participants, plus the count of distinct registered handlers.
    /// </summary>
    public class InfrastructureHealthV2
    {
        /// <summary>
        /// Gets or sets the total number of registered event addresses.
        /// </summary>
        public long TotalEventAddresses { get; set; }

        /// <summary>
        /// Gets or sets the total number of registered event listeners.
        /// </summary>
        public long TotalEventListeners { get; set; }

        /// <summary>
        /// Gets or sets the total number of registered participants.
        /// </summary>
        public long TotalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the count of distinct handler identifiers across all registered listeners.
        /// </summary>
        public long RegisteredHandlers { get; set; }

        /// <summary>
        /// Gets or sets the RAG status for the registered-handlers tile. Computed by the coordination.
        /// </summary>
        public HealthStatusV2 HandlerStatus { get; set; }
    }
}
