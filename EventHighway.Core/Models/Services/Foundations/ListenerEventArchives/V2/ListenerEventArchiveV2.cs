// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2
{
    /// <summary>
    /// Represents an archived record of an event being delivered to a listener, including the delivery status and response.
    /// </summary>
    public class ListenerEventArchiveV2
    {
        /// <summary>
        /// Gets or sets the unique identifier of the listener event archive.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the originating listener event this record was archived from.
        /// A null value indicates the archived listener event was not produced by a replay.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ListenerEventArchiveStatusV2"/> indicating the current state of the archived listener event delivery.
        /// </summary>
        public ListenerEventArchiveStatusV2 Status { get; set; }

        /// <summary>
        /// Gets or sets the serialized response body received from the listener.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets the HTTP response code returned by the listener.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the response message or status text returned by the listener.
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this listener event archive was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this listener event archive was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the number of additional dispatch attempts remaining after the initial dispatch,
        /// as recorded at the time this delivery was archived.
        /// </summary>
        public int RemainingRetryAttempts { get; set; }

        /// <summary>
        /// Gets or sets the attempts ceiling for this delivery, as recorded at the time this delivery was archived.
        /// </summary>
        public int RetryAttemptsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the earliest time the next retry was permitted to run, as recorded at the time this
        /// delivery was archived. A null value indicates the delivery was eligible immediately.
        /// </summary>
        public DateTimeOffset? NextRetryAttemptNotBefore { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the most recent actual dispatch, as recorded at the time this delivery
        /// was archived. Null indicates the delivery had never been dispatched.
        /// </summary>
        public DateTimeOffset? DispatchedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this listener event archive was archived.
        /// </summary>
        public DateTimeOffset ArchivedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event associated with this archived listener event.
        /// </summary>
        public Guid EventV2Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event address where the archived event was sent.
        /// </summary>
        public Guid EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event listener that received the archived event.
        /// </summary>
        public Guid EventListenerV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventListenerV2"/> navigation property associated with this listener event archive.
        /// </summary>
        public EventListenerV2 EventListenerV2 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the archived event.
        /// </summary>
        public Guid EventArchiveV2Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="EventParticipants.V2.EventParticipantV2"/> associated with this listener event archive.
        /// A null value indicates no participant is associated with this listener event archive.
        /// </summary>
        public Guid? EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventParticipants.V2.EventParticipantV2"/> navigation property associated with this listener event archive.
        /// </summary>
        public EventParticipantV2 EventParticipantV2 { get; set; }
    }
}
