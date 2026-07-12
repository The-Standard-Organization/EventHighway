// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives.Exceptions
{
    public class ListenerEventArchivesViewDependencyValidationException : Xeption
    {
        public ListenerEventArchivesViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Listener event archives view dependency validation error occurred, fix the errors and try again.",
                innerException)
        { }
    }
}
