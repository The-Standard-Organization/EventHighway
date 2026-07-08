// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Foundations.UserEventParticipants
{
    public class UserEventParticipant
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventParticipantId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
