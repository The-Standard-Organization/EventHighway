// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.SubstrateApp.Models.Events
{
    /// <summary>
    /// A generic wrapper that carries a domain event payload alongside the addressing,
    /// attribution and scheduling details required to publish it onto the event substrate.
    /// Services emit typed content through <c>IEventSubstrateBroker.EmitAsync</c> without
    /// composing raw <c>EventV2</c> records or serializing payloads themselves.
    /// </summary>
    /// <typeparam name="TContent">The type of the domain event content payload.</typeparam>
    public sealed class EventEnvelope<TContent>
    {
        /// <summary>
        /// Minted up-front so callers can hold on to the identifier for later replay.
        /// </summary>
        public Guid EventId { get; init; } = Guid.NewGuid();

        public string EventName { get; init; } = string.Empty;

        /// <summary>
        /// The business payload of the event, such as the domain entity that was
        /// created, updated, or deleted.
        /// </summary>
        public TContent Content { get; init; } = default!;

        public Guid EventAddressId { get; init; }

        /// <summary>
        /// The publishing participant's identity and secret, verified by the substrate.
        /// </summary>
        public Guid ParticipantId { get; init; }

        public string Secret { get; init; } = string.Empty;

        public DateTimeOffset OccurredAt { get; init; }

        /// <summary>
        /// When set, the event is held on the substrate until this moment; otherwise
        /// it is dispatched immediately.
        /// </summary>
        public DateTimeOffset? ScheduledDate { get; init; }
    }
}
