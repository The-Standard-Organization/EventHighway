// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Portal.Web.Models.Brokers.EventHighways
{
    // Flat view-model of an EventArchiveV2 with its aggregate listener-event-archive counts.
    // The archive rows and listener-event-archive rows are retrieved separately and joined
    // in-memory by the view service, keeping the client boundary free of IQueryable.
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
