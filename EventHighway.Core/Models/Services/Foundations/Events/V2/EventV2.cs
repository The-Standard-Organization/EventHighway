// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Models.Services.Foundations.Events.V2
{
    /// <summary>
    /// Represents an event in the V2 service model, containing event data, metadata, and
    /// relationship information to event addresses and listeners.
    /// </summary>
    public class EventV2
    {
        /// <summary>
        /// Gets or sets the unique identifier for this event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the content payload of this event.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the name of this event.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the hash of the content payload of this event.
        /// </summary>
        public string ContentHash { get; set; }

        /// <summary>
        /// Gets or sets the type of this event, such as scheduled, immediate, or
        /// recurring.
        /// </summary>
        public EventTypeV2 Type { get; set; }

        /// <summary>
        /// Gets or sets the status of this event. Defaults to Active.
        /// </summary>
        public EventStatusV2 Status { get; set; } = EventStatusV2.Active;

        /// <summary>
        /// Gets or sets the date and time when this event was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the optional date and time when this event is scheduled to be
        /// processed. If null, the event is processed immediately.
        /// </summary>
        public DateTimeOffset? ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts remaining for this event if processing
        /// fails.
        /// </summary>
        public int RemainingRetryAttempts { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the event address associated with this
        /// event.
        /// </summary>
        public Guid EventAddressId { get; set; }

        /// <summary>
        /// Gets or sets the event address object that this event is associated with.
        /// </summary>
        public EventAddressV2 EventAddressV2 { get; set; }

        /// <summary>
        /// Gets or sets the collection of listener events that track how listeners
        /// processed this event.
        /// </summary>
        public IEnumerable<ListenerEventV2> ListenerEventV2s { get; set; }
    }
}
