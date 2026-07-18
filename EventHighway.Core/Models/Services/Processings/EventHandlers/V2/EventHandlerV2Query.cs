// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Services.Processings.EventHandlers.V2
{
    /// <summary>
    /// Represents the search criteria for querying V2 event handlers (the union of the
    /// in-process registry and the persisted registrations). Every criterion is optional;
    /// omitted criteria are not applied. Results are ordered by <c>Name</c> then <c>Id</c> and
    /// paged with <see cref="Skip"/> and <see cref="Take"/>.
    /// </summary>
    public class EventHandlerV2Query
    {
        /// <summary>
        /// Gets or sets the event handler name to filter by (exact match), when set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of matching event handlers to skip. Defaults to zero.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of event handlers to return. Defaults to 100.
        /// </summary>
        public int Take { get; set; } = 100;
    }
}
