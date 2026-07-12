// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants.Exceptions
{
    public class EventParticipantsViewDependencyValidationException : Xeption
    {
        public EventParticipantsViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Event participants view dependency validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
