// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2
{
    /// <summary>
    /// Represents the search criteria for querying V2 events. Every criterion is optional;
    /// omitted criteria are not applied. Results are ordered by <c>CreatedDate</c> descending
    /// (most recent first) and paged with <see cref="Skip"/> and <see cref="Take"/>.
    /// </summary>
    public class EventV2Query
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
        /// Gets or sets the event status to filter by, when set.
        /// </summary>
        public EventStatusV2? Status { get; set; }

        /// <summary>
        /// Gets or sets the event type to filter by, when set.
        /// </summary>
        public EventTypeV2? Type { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedTo { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>ScheduledDate</c>, when set.
        /// Events without a scheduled date (immediate events) are excluded.
        /// </summary>
        public DateTimeOffset? ScheduledFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>ScheduledDate</c>, when set.
        /// Events without a scheduled date (immediate events) are excluded.
        /// </summary>
        public DateTimeOffset? ScheduledTo { get; set; }

        /// <summary>
        /// Gets or sets the number of matching events to skip. Defaults to zero.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of events to return. Defaults to 100.
        /// </summary>
        public int Take { get; set; } = 100;
    }
}
