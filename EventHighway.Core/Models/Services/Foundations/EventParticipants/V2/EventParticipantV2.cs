// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2
{
    /// <summary>
    /// Represents a participant that can publish or subscribe to events within the EventHighway system.
    /// </summary>
    public class EventParticipantV2
    {
        /// <summary>
        /// Gets or sets the unique identifier of the participant.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the participant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a human-readable description of the participant.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the contact email address for the participant.
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the contact phone number for the participant.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this participant is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a valid secret is mandatory when
        /// submitting events on behalf of this participant.
        /// </summary>
        public bool IsSecretRequired { get; set; }

        /// <summary>
        /// Gets or sets the date and time from which this participant becomes active.
        /// A null value indicates no lower bound on the active period.
        /// </summary>
        public DateTimeOffset? ActiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the date and time after which this participant is no longer active.
        /// A null value indicates no expiry.
        /// </summary>
        public DateTimeOffset? ActiveTo { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this participant was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this participant was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="EventV2"/> events published by this participant.
        /// </summary>
        public IEnumerable<EventV2> EventV2s { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="EventArchiveV2"/> archived events associated with this participant.
        /// </summary>
        public IEnumerable<EventArchiveV2> EventArchiveV2s { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="EventListenerV2"/> listeners owned by this participant.
        /// </summary>
        public IEnumerable<EventListenerV2> EventListenerV2s { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="ListenerEventV2"/> listener events associated with this participant.
        /// </summary>
        public IEnumerable<ListenerEventV2> ListenerEventV2s { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="ListenerEventArchiveV2"/> archived listener events associated with this participant.
        /// </summary>
        public IEnumerable<ListenerEventArchiveV2> ListenerEventArchiveV2s { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="EventParticipantSecretV2"/> secrets belonging to this participant.
        /// </summary>
        public IEnumerable<EventParticipantSecretV2> EventParticipantSecretV2s { get; set; }
    }
}
