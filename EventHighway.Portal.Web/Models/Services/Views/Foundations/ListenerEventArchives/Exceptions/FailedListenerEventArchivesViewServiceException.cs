// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives.Exceptions
{
    public class FailedListenerEventArchivesViewServiceException : Xeption
    {
        public FailedListenerEventArchivesViewServiceException(Exception innerException)
            : base(
                message: "Failed listener event archives view service error occurred, contact support.",
                innerException)
        { }
    }
}
