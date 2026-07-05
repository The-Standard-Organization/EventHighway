// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Configurations.Purging
{
    /// <summary>
    /// Configures retention for the scheduled purge of archived events.
    /// </summary>
    public class PurgeConfiguration
    {
        /// <summary>
        /// Gets or sets the number of days an archived event is retained before the scheduled
        /// purge permanently deletes it. Defaults to <c>1825</c> (5 years).
        /// </summary>
        public int RetentionDays { get; set; } = 1825;
    }
}
