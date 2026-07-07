// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the retry breakdown for a single event address within a
    /// <see cref="RetryHealthSummaryV2"/>.
    /// </summary>
    public class RetryAddressDetailV2
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
        /// Gets or sets the number of events on this address with zero remaining retry attempts.
        /// </summary>
        public long DeadEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events on this address with one or two remaining retry attempts.
        /// </summary>
        public long CriticalEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events on this address with three or more remaining retry attempts.
        /// </summary>
        public long HealthyEvents { get; set; }

        /// <summary>
        /// Gets or sets the total number of errored listener events on this address within the window.
        /// </summary>
        public long TotalEvents { get; set; }
    }
}
