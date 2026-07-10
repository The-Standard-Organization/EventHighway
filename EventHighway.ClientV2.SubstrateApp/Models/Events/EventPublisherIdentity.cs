// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.SubstrateApp.Models.Events
{
    /// <summary>
    /// The participant identity this application presents when publishing its own events onto
    /// the substrate (as opposed to external contributions, which carry the contributing
    /// participant's credentials). The substrate core verifies the secret on every emission.
    /// </summary>
    public sealed class EventPublisherIdentity
    {
        public Guid ParticipantId { get; init; }
        public string Secret { get; init; } = string.Empty;
    }
}
