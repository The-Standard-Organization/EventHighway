// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives.Exceptions
{
    public class ListenerEventArchivesViewDependencyException : Xeption
    {
        public ListenerEventArchivesViewDependencyException(Xeption innerException)
            : base(
                message: "Listener event archives view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
