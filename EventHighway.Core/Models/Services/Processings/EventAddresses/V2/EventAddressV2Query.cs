// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Processings.EventAddresses.V2
{
    /// <summary>
    /// Represents the search criteria for querying V2 event addresses. Every criterion is
    /// optional; omitted criteria are not applied. Results are ordered by <c>CreatedDate</c>
    /// descending (most recent first) and paged with <see cref="Skip"/> and <see cref="Take"/>.
    /// </summary>
    public class EventAddressV2Query
    {
        /// <summary>
        /// Gets or sets the event address name to filter by (exact match), when set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedTo { get; set; }

        /// <summary>
        /// Gets or sets the number of matching event addresses to skip. Defaults to zero.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of event addresses to return. Defaults to 100.
        /// </summary>
        public int Take { get; set; } = 100;
    }
}
