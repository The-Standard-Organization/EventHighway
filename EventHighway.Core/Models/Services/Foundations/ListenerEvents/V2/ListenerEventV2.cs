// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2
{
    /// <summary>
    /// Represents a record of an event being delivered to a listener, including the delivery status and response.
    /// </summary>
    public class ListenerEventV2
    {
        /// <summary>
        /// Gets or sets the unique identifier of the listener event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the originating listener event this record was replayed from.
        /// When a listener event is restored from an archive during replay it receives a new <see cref="Id"/>,
        /// and this correlation identifier is set to the archived listener event's identifier to preserve history.
        /// A null value indicates the listener event was not produced by a replay.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ListenerEventStatusV2"/> indicating the current state of the listener event delivery.
        /// </summary>
        public ListenerEventStatusV2 Status { get; set; }

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
        /// Gets or sets the date and time when this listener event was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this listener event was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the number of additional dispatch attempts remaining after the initial dispatch.
        /// A value of <c>0</c> indicates the delivery is dead (retry budget exhausted).
        /// </summary>
        public int RemainingRetryAttempts { get; set; }

        /// <summary>
        /// Gets or sets the attempts ceiling for this delivery. The Fibonacci backoff index is
        /// <c>RetryAttemptsAllowed − RemainingRetryAttempts</c>. Seeded from configuration and grown on extend.
        /// </summary>
        public int RetryAttemptsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the earliest time the next retry may run (the Fibonacci backoff gate).
        /// A null value indicates the delivery is eligible now.
        /// </summary>
        public DateTimeOffset? NextRetryAttemptNotBefore { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the most recent actual dispatch. It slides forward on each retry and
        /// anchors the dead/archive grace window. Null until first dispatched (e.g. freshly-restored replay rows).
        /// </summary>
        public DateTimeOffset? DispatchedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="Events.V2.EventV2"/> being delivered.
        /// </summary>
        public Guid EventV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Events.V2.EventV2"/> navigation property being delivered to this listener.
        /// </summary>
        public EventV2 EventV2 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="EventAddresses.V2.EventAddressV2"/> where the event was sent.
        /// </summary>
        public Guid EventAddressV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventAddresses.V2.EventAddressV2"/> navigation property representing the delivery endpoint.
        /// </summary>
        public EventAddressV2 EventAddressV2 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="EventListeners.V2.EventListenerV2"/> receiving the event.
        /// </summary>
        public Guid EventListenerV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventListeners.V2.EventListenerV2"/> navigation property that represents the listener.
        /// </summary>
        public EventListenerV2 EventListenerV2 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="EventParticipants.V2.EventParticipantV2"/> associated with this listener event.
        /// Required: every listener event is attributed to the participant that owns its listener.
        /// </summary>
        public Guid EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventParticipants.V2.EventParticipantV2"/> navigation property associated with this listener event.
        /// </summary>
        public EventParticipantV2 EventParticipantV2 { get; set; }
    }
}
