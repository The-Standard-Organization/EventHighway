// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents a section of the health report that failed to load, so the rest of the
    /// report can still render (per-section resilience).
    /// </summary>
    public class HealthSectionErrorV2
    {
        /// <summary>
        /// Gets or sets the name of the failed section (for example "Traffic" or "Retry").
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// Gets or sets the generic, user-safe message describing the failure.
        /// </summary>
        public string Message { get; set; }
    }
}
