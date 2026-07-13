// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers.Exceptions
{
    public class EventHandlersViewDependencyException : Xeption
    {
        public EventHandlersViewDependencyException(Xeption innerException)
            : base(
                message: "Event handlers view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
