// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners.Exceptions
{
    public class EventListenersViewDependencyException : Xeption
    {
        public EventListenersViewDependencyException(Xeption innerException)
            : base(
                message: "Event listeners view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
