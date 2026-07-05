// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Configurations.Purging;
using EventHighway.Core.Models.Configurations.Retries;

namespace EventHighway.Core.Models.Configurations
{
    /// <summary>
    /// Root configuration object passed to the EventHighway client at construction time.
    /// All sections default to safe, out-of-the-box values so the object is optional.
    /// </summary>
    public class EventHighwayConfiguration
    {
        /// <summary>
        /// Gets or sets the health-check configuration, including RAG thresholds for each metric.
        /// Defaults to a <see cref="HealthConfiguration"/> with standard thresholds pre-populated.
        /// </summary>
        public HealthConfiguration Health { get; set; } = new HealthConfiguration();

        /// <summary>
        /// Gets or sets the batch processing configuration, including the bulk-operation batch size.
        /// Defaults to a <see cref="BatchConfiguration"/> with system defaults.
        /// </summary>
        public BatchConfiguration BatchProcessing { get; set; } = new BatchConfiguration();

        /// <summary>
        /// Gets or sets the loop detection configuration for identifying and managing recursive event patterns.
        /// Defaults to a <see cref="LoopDetection"/> with standard thresholds.
        /// </summary>
        public LoopDetection LoopDetection { get; set; } = new LoopDetection();

        /// <summary>
        /// Gets or sets the retry configuration for durable, spaced-out re-dispatch of failed listener deliveries.
        /// Defaults to a <see cref="RetryConfiguration"/> with standard budget and backoff values.
        /// </summary>
        public RetryConfiguration RetryConfiguration { get; set; } = new RetryConfiguration();

        /// <summary>
        /// Gets or sets the purge configuration that controls how long archived events are retained
        /// before the scheduled purge permanently deletes them.
        /// Defaults to a <see cref="PurgeConfiguration"/> with a standard retention window.
        /// </summary>
        public PurgeConfiguration Purging { get; set; } = new PurgeConfiguration();
    }
}
