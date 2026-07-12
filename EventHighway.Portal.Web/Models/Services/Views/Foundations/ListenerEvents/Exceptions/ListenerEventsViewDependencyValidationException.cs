// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents.Exceptions
{
    public class ListenerEventsViewDependencyValidationException : Xeption
    {
        public ListenerEventsViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Listener events view dependency validation error occurred, fix the errors and try again.",
                innerException)
        { }
    }
}
