// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the per-participant usage roll-up within the report window, combining publishing
    /// activity and listener ownership. Rows are merged by id in the coordination: the infrastructure
    /// orchestration contributes the name-bearing fields, the events orchestration the counts.
    /// </summary>
    public class ParticipantUsageV2
    {
        /// <summary>
        /// Gets or sets the identifier of the participant.
        /// </summary>
        public Guid EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the participant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the participant is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the number of events this participant submitted within the window.
        /// </summary>
        public long TotalEventsSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct event addresses this participant published to within the window.
        /// </summary>
        public long ActiveEventAddresses { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events on listeners owned by this participant within the window.
        /// </summary>
        public long TotalListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listeners owned by this participant.
        /// </summary>
        public long OwnedListeners { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events on listeners owned by this participant that
        /// ended in an error state within the window.
        /// </summary>
        public long ErrorListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the percentage of errors on events this participant submitted.
        /// </summary>
        public decimal PublisherErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the percentage of errors on listeners this participant owns.
        /// </summary>
        public decimal ListenerErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the number of loop detections attributed to this participant within the window.
        /// </summary>
        public long LoopsDetected { get; set; }

        /// <summary>
        /// Gets or sets the number of duplicates attributed to this participant within the window.
        /// </summary>
        public long DuplicatesDetected { get; set; }

        /// <summary>
        /// Gets or sets the per-address sent/received breakdown for this participant within the window.
        /// </summary>
        public IReadOnlyList<ParticipantAddressUsageV2> ByAddress { get; set; }

        /// <summary>
        /// Gets or sets the overall RAG status computed for this participant row. Computed by the coordination.
        /// </summary>
        public HealthStatusV2 Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the participant's most recent activity, or
        /// <c>null</c> when there was no activity within the window.
        /// </summary>
        public DateTimeOffset? LastActivity { get; set; }
    }
}
