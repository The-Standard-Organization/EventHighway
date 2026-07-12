// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners.Exceptions
{
    public class EventListenersViewDependencyValidationException : Xeption
    {
        public EventListenersViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Event listeners view dependency validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
