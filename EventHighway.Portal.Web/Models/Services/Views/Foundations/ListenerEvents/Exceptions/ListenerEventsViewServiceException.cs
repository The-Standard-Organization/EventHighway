// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents.Exceptions
{
    public class ListenerEventsViewServiceException : Xeption
    {
        public ListenerEventsViewServiceException(Xeption innerException)
            : base(
                message: "Listener events view service error occurred, contact support.",
                innerException)
        { }
    }
}
