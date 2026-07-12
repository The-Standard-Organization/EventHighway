// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers.Exceptions
{
    public class EventHandlersViewDependencyValidationException : Xeption
    {
        public EventHandlersViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Event handlers view dependency validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
