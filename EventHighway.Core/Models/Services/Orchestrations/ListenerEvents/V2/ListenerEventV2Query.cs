// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2
{
    /// <summary>
    /// Represents the search criteria for querying V2 listener events. Every criterion is
    /// optional; omitted criteria are not applied. Results are ordered by <c>CreatedDate</c>
    /// descending (most recent first) and paged with <see cref="Skip"/> and <see cref="Take"/>.
    /// </summary>
    public class ListenerEventV2Query
    {
        /// <summary>
        /// Gets or sets the delivery status to filter by, when set.
        /// </summary>
        public ListenerEventStatusV2? Status { get; set; }

        /// <summary>
        /// Gets or sets the originating event to filter by, when set.
        /// </summary>
        public Guid? EventV2Id { get; set; }

        /// <summary>
        /// Gets or sets the event address to filter by, when set.
        /// </summary>
        public Guid? EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the event listener to filter by, when set.
        /// </summary>
        public Guid? EventListenerV2Id { get; set; }

        /// <summary>
        /// Gets or sets the participant to filter by, when set.
        /// </summary>
        public Guid? EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the correlation id to filter by, when set.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the inclusive lower bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedFrom { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound on <c>CreatedDate</c>, when set.
        /// </summary>
        public DateTimeOffset? CreatedTo { get; set; }

        /// <summary>
        /// Gets or sets the number of matching listener events to skip. Defaults to zero.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of listener events to return. Defaults to 100.
        /// </summary>
        public int Take { get; set; } = 100;
    }
}
