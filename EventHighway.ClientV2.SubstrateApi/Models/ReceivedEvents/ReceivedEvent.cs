// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents
{
    /// <summary>
    /// One delivery that arrived on the /receive endpoint: what came in, and when. The chat UI
    /// renders these newest-last, one per turn.
    /// </summary>
    public sealed class ReceivedEvent
    {
        public Guid Id { get; init; }

        public DateTimeOffset ReceivedDate { get; init; }

        /// <summary>
        /// Exactly what was POSTed — kept verbatim, whatever shape it was in.
        /// </summary>
        public string Content { get; init; } = string.Empty;

        /// <summary>
        /// The same content re-written with indentation for display; falls back to
        /// <see cref="Content"/> when what arrived was not JSON at all.
        /// </summary>
        public string FormattedContent { get; init; } = string.Empty;
    }
}
