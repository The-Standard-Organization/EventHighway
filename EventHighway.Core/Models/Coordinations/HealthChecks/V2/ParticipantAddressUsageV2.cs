// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents a participant's per-address activity split within the report window:
    /// events the participant submitted on the address and deliveries received by listeners it owns.
    /// </summary>
    public class ParticipantAddressUsageV2
    {
        /// <summary>
        /// Gets or sets the identifier of the event address.
        /// </summary>
        public Guid EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the event address.
        /// </summary>
        public string EventAddressV2Name { get; set; }

        /// <summary>
        /// Gets or sets the number of events the participant submitted on this address within the window.
        /// </summary>
        public long Sent { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Sent"/> as a percentage (0–100) of all events sent overall
        /// (across all participants and addresses) within the window. Computed by the coordination.
        /// </summary>
        public decimal SentPercentage { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events on listeners the participant owns
        /// on this address within the window.
        /// </summary>
        public long Received { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Received"/> as a percentage (0–100) of all listener events received
        /// overall (across all participants and addresses) within the window. Computed by the coordination.
        /// </summary>
        public decimal ReceivedPercentage { get; set; }
    }
}
