// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents.Exceptions
{
    public class ListenerEventsViewDependencyException : Xeption
    {
        public ListenerEventsViewDependencyException(Xeption innerException)
            : base(
                message: "Listener events view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
