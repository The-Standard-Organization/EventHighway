// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the distribution of remaining retry attempts across errored listener events
    /// within the report window, used to drive the retry health histogram.
    /// </summary>
    public class RetryHealthSummaryV2
    {
        /// <summary>
        /// Gets or sets the period granularity the summary was aggregated over.
        /// </summary>
        public TrafficPeriodV2 Period { get; set; }

        /// <summary>
        /// Gets or sets the inclusive UTC start of the window.
        /// </summary>
        public DateTimeOffset WindowStart { get; set; }

        /// <summary>
        /// Gets or sets the exclusive UTC end of the window.
        /// </summary>
        public DateTimeOffset WindowEnd { get; set; }

        /// <summary>
        /// Gets or sets the human-readable label describing the window.
        /// </summary>
        public string WindowLabel { get; set; }

        /// <summary>
        /// Gets or sets the number of errored listener events still eligible for retry
        /// (a remaining retry attempt greater than zero) within the window.
        /// </summary>
        public long TotalActiveEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events with zero remaining retry attempts.
        /// </summary>
        public long DeadEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events with one or two remaining retry attempts.
        /// </summary>
        public long CriticalEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events with three or more remaining retry attempts.
        /// </summary>
        public long HealthyEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of errored archived listener events with no remaining retry attempts
        /// (terminal dead deliveries) within the window. Contributed by the archived-events orchestration.
        /// </summary>
        public long ArchivedDeadEvents { get; set; }

        /// <summary>
        /// Gets or sets the histogram buckets keyed by remaining retry count.
        /// </summary>
        public IEnumerable<RetryBucketV2> Distribution { get; set; }

        /// <summary>
        /// Gets or sets the per-address retry breakdown.
        /// </summary>
        public IEnumerable<RetryAddressDetailV2> ByAddress { get; set; }
    }
}
