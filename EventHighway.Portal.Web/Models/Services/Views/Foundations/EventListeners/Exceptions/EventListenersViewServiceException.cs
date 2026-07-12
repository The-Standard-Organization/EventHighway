// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners.Exceptions
{
    public class EventListenersViewServiceException : Xeption
    {
        public EventListenersViewServiceException(Xeption innerException)
            : base(
                message: "Event listeners view service error occurred, contact support.",
                innerException)
        { }
    }
}
