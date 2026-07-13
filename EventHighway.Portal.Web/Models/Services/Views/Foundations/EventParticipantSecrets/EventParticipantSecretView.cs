// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets
{
    public class EventParticipantSecretView
    {
        public Guid Id { get; set; }
        public string Secret { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTimeOffset? ActiveFrom { get; set; }
        public DateTimeOffset? ActiveTo { get; set; }
        public Guid EventParticipantV2Id { get; set; }
    }
}
