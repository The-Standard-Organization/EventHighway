// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2
{
    /// <summary>
    /// Represents a secret credential associated with an event participant, used for authentication and access control.
    /// </summary>
    public class EventParticipantSecretV2
    {
        /// <summary>
        /// Gets or sets the unique identifier of the secret.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the secret value used for authentication. This is write-once: on Add the
        /// supplied plaintext is replaced in place with its SHA-256 hash, and every subsequent
        /// retrieval returns that hash — never the original plaintext. The value is immutable after
        /// creation; rotate a secret by removing it and adding a new one.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this secret is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the date and time from which this secret becomes valid.
        /// A null value indicates no lower bound on validity.
        /// </summary>
        public DateTimeOffset? ActiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the date and time after which this secret is no longer valid.
        /// A null value indicates no expiry.
        /// </summary>
        public DateTimeOffset? ActiveTo { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this secret was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this secret was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="EventParticipantV2"/> this secret belongs to.
        /// </summary>
        public Guid EventParticipantV2Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventParticipantV2"/> navigation property this secret belongs to.
        /// </summary>
        public EventParticipantV2 EventParticipantV2 { get; set; }
    }
}
