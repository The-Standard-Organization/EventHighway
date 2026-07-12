// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives.Exceptions
{
    public class EventArchivesViewDependencyValidationException : Xeption
    {
        public EventArchivesViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Event archives view dependency validation error occurred, fix the errors and try again.",
                innerException)
        { }
    }
}
