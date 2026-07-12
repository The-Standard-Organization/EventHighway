// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers.Exceptions
{
    public class EventHandlersViewServiceException : Xeption
    {
        public EventHandlersViewServiceException(Xeption innerException)
            : base(
                message: "Event handlers view service error occurred, contact support.",
                innerException)
        { }
    }
}
