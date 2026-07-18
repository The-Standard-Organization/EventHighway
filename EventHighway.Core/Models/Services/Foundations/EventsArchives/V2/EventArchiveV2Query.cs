// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2
{
    /// <summary>
    /// Represents the search criteria for querying V2 event archives. Every criterion is
    /// optional; omitted criteria are not applied. Results are ordered by <c>ArchivedDate</c>
    /// descending (most recently archived first) and paged with <see cref="Skip"/> and
    /// <see cref="Take"/>.
    /// </summary>
    public class EventArchiveV2Query
    {
        /// <summary>
        /// Gets or sets the event address to filter by, when set.
        /// </summary>
        public Guid? EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the submitting participant to filter by, when set.
        /// </summary>
        public Guid? EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the event name to filter by (exact match), when set.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the archived event status to filter by, when set.
        /// </summary>
        public EventArchiveStatusV2? Status { get; set; }

        /// <summary>
        /// Gets or sets the archived event type to filter by, when set.
        /// </summary>
        public EventArchiveTypeV2? Type { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedTo { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>ArchivedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? ArchivedFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>ArchivedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? ArchivedTo { get; set; }

        /// <summary>
        /// Gets or sets the number of matching archived events to skip. Defaults to zero.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of archived events to return. Defaults to 100.
        /// </summary>
        public int Take { get; set; } = 100;
    }
}
