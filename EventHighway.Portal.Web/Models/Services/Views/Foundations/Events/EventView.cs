// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Events
{
    public class EventView
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string EventAddressName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ListenerEventCount { get; set; }
        public int SucceededListenerEventCount { get; set; }
        public Guid EventAddressV2Id { get; set; }
        public Guid? EventParticipantV2Id { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        // Aggregate delivery outcome across the event's listener events: all succeeded ->
        // Success, all failed -> Error, some of each -> Partial Success. Quarantined events
        // are never dispatched and events with no listener events yet are Pending.
        public string DispatchStatus =>
            Status == "Quarantined" ? "Quarantined"
                : ListenerEventCount == 0 ? "Pending"
                : SucceededListenerEventCount == ListenerEventCount ? "Success"
                : SucceededListenerEventCount == 0 ? "Error"
                : "Partial Success";

        public string Processed =>
            $"{SucceededListenerEventCount}/{ListenerEventCount}";
    }
}
