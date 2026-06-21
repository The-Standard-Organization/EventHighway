// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Configurations.LoopDetections
{
    /// <summary>
    /// Loop detection configuration for identifying and managing recursive event patterns.
    /// </summary>
    public class LoopDetection
    {
        /// <summary>
        /// Gets or sets a value indicating whether loop detection is enabled.
        /// Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum occurrence threshold within the detection window before triggering a loop alert.
        /// Defaults to 5.
        /// </summary>
        public int Threshold { get; set; } = 5;

        /// <summary>
        /// Gets or sets the time window for tracking event occurrences.
        /// Defaults to 60 seconds.
        /// </summary>
        public TimeSpan Window { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>Per-event-address volatile content paths. Only addresses listed here have
        /// fields stripped before hashing.</summary>
        public List<VolatilePaths> VolatilePaths { get; set; } = new List<VolatilePaths>();
    }
}
