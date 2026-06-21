// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Configurations.LoopDetections
{
    /// <summary>
    /// Per-event-address volatile content path configuration.
    /// </summary>
    public class VolatilePaths
    {
        /// <summary>Gets or sets the unique identifier for this event address.</summary>
        public Guid EventAddressId { get; set; }

        /// <summary>Gets or sets the name of this event address.</summary>
        public string Name { get; set; }

        /// <summary>Content fields excluded from the hash because they change on every repost
        /// (audit/transport fields). Defaults to common audit fields.</summary>
        public string[] VolatileContentPaths { get; set; } =
            new[] { "CreatedBy", "CreatedWhen", "UpdatedBy", "UpdatedWhen", "CreatedDate", "UpdatedDate" };
    }
}
