// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2
{
    /// <summary>
    /// Represents an archived event that has been processed and stored for historical records and audit purposes.
    /// </summary>
    public class EventArchiveV2
    {
        /// <summary>
        /// Gets or sets the unique identifier of the archived event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the serialized content or payload of the archived event.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the name or type of the event.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventArchiveTypeV2"/> that categorizes this archived event.
        /// </summary>
        public EventArchiveTypeV2 Type { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventArchiveStatusV2"/> indicating the current state of the archived event.
        /// Defaults to <see cref="EventArchiveStatusV2.Active"/>.
        /// </summary>
        public EventArchiveStatusV2 Status { get; set; } = EventArchiveStatusV2.Active;

        /// <summary>
        /// Gets or sets the date and time when this event was originally created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this archived event was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event is scheduled to be processed.
        /// A null value indicates the event is not scheduled for future processing.
        /// </summary>
        public DateTimeOffset? ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event was archived.
        /// </summary>
        public DateTimeOffset ArchivedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event address associated with this archived event.
        /// </summary>
        public Guid EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventAddressV2"/> navigation property associated with this archived event.
        /// </summary>
        public EventAddressV2 EventAddressV2 { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="ListenerEventArchiveV2"/> listener events associated with this archived event.
        /// </summary>
        public IEnumerable<ListenerEventArchiveV2> ListenerEventArchiveV2s { get; set; }

        /// <summary>
        /// Gets or sets the secret associated with the participant for this event.
        /// </summary>
        public string EventParticipantV2Secret { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="EventParticipants.V2.EventParticipantV2"/> that created or is responsible for this archived event.
        /// A null value indicates no participant is associated with this event.
        /// </summary>
        public Guid? EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventParticipants.V2.EventParticipantV2"/> navigation property associated with this archived event.
        /// </summary>
        public EventParticipantV2 EventParticipantV2 { get; set; }
    }
}
