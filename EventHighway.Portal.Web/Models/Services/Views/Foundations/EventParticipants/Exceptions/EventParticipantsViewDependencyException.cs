// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants.Exceptions
{
    public class EventParticipantsViewDependencyException : Xeption
    {
        public EventParticipantsViewDependencyException(Xeption innerException)
            : base(
                message: "Event participants view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
