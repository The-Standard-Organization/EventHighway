// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Events.Exceptions
{
    public class EventsViewServiceException : Xeption
    {
        public EventsViewServiceException(Xeption innerException)
            : base(
                message: "Events view service error occurred, contact support.",
                innerException)
        { }
    }
}
