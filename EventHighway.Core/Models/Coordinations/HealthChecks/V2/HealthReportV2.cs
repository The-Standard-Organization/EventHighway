// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// The health virtual model — the single shared contract of the health orchestrations and the
    /// health coordination. Each orchestration returns it partially populated (only the children its
    /// own foundations produce); the coordination merges the partials into one complete report; each
    /// health sub-client flattens out exactly one child.
    /// </summary>
    public class HealthReportV2
    {
        /// <summary>
        /// Gets or sets the period granularity the windowed children were aggregated over.
        /// Whole-system RAG items ignore the window.
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
        /// Gets or sets the UTC timestamp the report was generated.
        /// </summary>
        public DateTimeOffset GeneratedDate { get; set; }

        /// <summary>
        /// Gets or sets the whole-system RAG tiles as generic items, extensible without contract change.
        /// Groupings: "Infrastructure", "Active Events", "Active Listeners", "Archived Events",
        /// "Archived Listeners". The UI drives its components off <see cref="HealthCheckItemV2.Grouping"/>.
        /// </summary>
        public IReadOnlyList<HealthCheckItemV2> HealthCheckItems { get; set; }

        /// <summary>
        /// Gets or sets the windowed traffic snapshot (live + archive combined).
        /// </summary>
        public TrafficSnapshotV2 Traffic { get; set; }

        /// <summary>
        /// Gets or sets the windowed per-address usage rows (live + archive combined, merged by id).
        /// </summary>
        public IReadOnlyList<EventAddressUsageV2> AddressUsage { get; set; }

        /// <summary>
        /// Gets or sets the windowed loop-detection summary (live + archive combined).
        /// </summary>
        public LoopDetectionSummaryV2 LoopDetection { get; set; }

        /// <summary>
        /// Gets or sets the windowed retry health summary.
        /// </summary>
        public RetryHealthSummaryV2 Retry { get; set; }

        /// <summary>
        /// Gets or sets the windowed per-participant usage rows (merged by id).
        /// </summary>
        public IReadOnlyList<ParticipantUsageV2> ParticipantUsage { get; set; }

        /// <summary>
        /// Gets or sets the windowed duplicate-detection summary (live only).
        /// </summary>
        public DuplicateDetectionSummaryV2 Duplicates { get; set; }

        /// <summary>
        /// Gets or sets the sections that failed to load, so the rest of the report can still render.
        /// Empty on full success.
        /// </summary>
        public IReadOnlyList<HealthSectionErrorV2> FailedSections { get; set; }
    }
}
