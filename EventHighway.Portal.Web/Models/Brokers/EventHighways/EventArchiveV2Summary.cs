// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Portal.Web.Models.Brokers.EventHighways
{
    // Flat EF projection of an EventArchiveV2 with its aggregate listener-event counts. The
    // counts are computed server-side (correlated COUNT subqueries) inside the broker's
    // database gate, so no listener-event rows are ever materialized regardless of volume.
    public class EventArchiveV2Summary
    {
        public Guid Id { get; init; }
        public string? EventName { get; init; }
        public string? Content { get; init; }
        public EventArchiveTypeV2 Type { get; init; }
        public EventArchiveStatusV2 Status { get; init; }
        public Guid EventAddressV2Id { get; init; }
        public string? EventAddressName { get; init; }
        public Guid? EventParticipantV2Id { get; init; }
        public DateTimeOffset? ScheduledDate { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset ArchivedDate { get; init; }
        public int ListenerEventCount { get; init; }
        public int SucceededListenerEventCount { get; init; }
    }
}
