// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives.Exceptions
{
    public class EventArchivesViewDependencyException : Xeption
    {
        public EventArchivesViewDependencyException(Xeption innerException)
            : base(
                message: "Event archives view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
