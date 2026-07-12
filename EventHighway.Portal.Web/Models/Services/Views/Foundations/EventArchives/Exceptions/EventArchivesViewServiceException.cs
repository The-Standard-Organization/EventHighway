// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives.Exceptions
{
    public class EventArchivesViewServiceException : Xeption
    {
        public EventArchivesViewServiceException(Xeption innerException)
            : base(
                message: "Event archives view service error occurred, contact support.",
                innerException)
        { }
    }
}
