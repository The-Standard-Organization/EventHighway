// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the per-event-address usage roll-up within the report window, combining live and
    /// archived volumes. Rows are merged by id in the coordination: the infrastructure orchestration
    /// contributes the name-bearing fields, the events/archived-events orchestrations the counts.
    /// </summary>
    public class EventAddressUsageV2
    {
        /// <summary>
        /// Gets or sets the identifier of the event address.
        /// </summary>
        public Guid EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the event address.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the event address.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of live events on this address within the window.
        /// </summary>
        public long TotalActiveEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of archived events on this address within the window.
        /// </summary>
        public long TotalArchivedEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of live listener events on this address within the window.
        /// </summary>
        public long TotalListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of archived listener events on this address within the window.
        /// </summary>
        public long TotalArchivedListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listeners currently registered against this address.
        /// </summary>
        public long ActiveListeners { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events on this address that ended in an error state
        /// within the window. On merged rows this combines live and archived counts.
        /// </summary>
        public long ErrorListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of dead deliveries (retry budget exhausted) on this address.
        /// </summary>
        public long DeadEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of quarantined (loop-detected) events on this address within the window.
        /// </summary>
        public long LoopsDetected { get; set; }

        /// <summary>
        /// Gets or sets the percentage of listener events on this address that ended in an error state.
        /// </summary>
        public decimal ErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the percentage of events on this address detected as content-hash duplicates.
        /// </summary>
        public decimal DuplicateRate { get; set; }

        /// <summary>
        /// Gets or sets the overall RAG status computed for this address row. Computed by the coordination.
        /// </summary>
        public HealthStatusV2 Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the most recent activity on this address, or
        /// <c>null</c> when there was no activity within the window.
        /// </summary>
        public DateTimeOffset? LastActivity { get; set; }
    }
}
