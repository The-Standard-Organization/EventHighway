// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants.Exceptions
{
    public class EventParticipantsViewServiceException : Xeption
    {
        public EventParticipantsViewServiceException(Xeption innerException)
            : base(
                message: "Event participants view service error occurred, contact support.",
                innerException)
        { }
    }
}
