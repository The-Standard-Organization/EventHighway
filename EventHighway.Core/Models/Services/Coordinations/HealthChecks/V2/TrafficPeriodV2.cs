// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Identifies the length of the traffic window a health summary is aggregated over,
    /// and the granularity of the buckets it is divided into.
    /// </summary>
    public enum TrafficPeriodV2
    {
        /// <summary>
        /// A 24-hour window divided into 24 hourly buckets.
        /// </summary>
        Day = 0,

        /// <summary>
        /// A 7-day window divided into 7 daily buckets.
        /// </summary>
        Week = 1,

        /// <summary>
        /// A one-month window divided into daily buckets (28-31 depending on the month).
        /// </summary>
        Month = 2,

        /// <summary>
        /// A one-year window divided into 12 monthly buckets.
        /// </summary>
        Year = 3,

        /// <summary>
        /// A caller-supplied window with an explicit end; bucket granularity is derived from the
        /// span (hourly up to 2 days, daily up to 62 days, monthly beyond).
        /// </summary>
        Custom = 4
    }
}
