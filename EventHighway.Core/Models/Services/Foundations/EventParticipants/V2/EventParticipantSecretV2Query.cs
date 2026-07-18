// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2
{
    /// <summary>
    /// Represents the search criteria for querying V2 event participant secrets. Every criterion
    /// is optional; omitted criteria are not applied. Results are ordered by <c>CreatedDate</c>
    /// descending (most recent first) and paged with <see cref="Skip"/> and <see cref="Take"/>.
    /// </summary>
    public class EventParticipantSecretV2Query
    {
        /// <summary>
        /// Gets or sets the owning participant to filter by, when set.
        /// </summary>
        public Guid? EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the active state to filter by, when set.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedTo { get; set; }

        /// <summary>
        /// Gets or sets the number of matching secrets to skip. Defaults to zero.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of secrets to return. Defaults to 100.
        /// </summary>
        public int Take { get; set; } = 100;
    }
}
