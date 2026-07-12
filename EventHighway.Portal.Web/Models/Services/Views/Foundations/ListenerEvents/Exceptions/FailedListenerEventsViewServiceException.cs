// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents.Exceptions
{
    public class FailedListenerEventsViewServiceException : Xeption
    {
        public FailedListenerEventsViewServiceException(Exception innerException)
            : base(
                message: "Failed listener events view service error occurred, contact support.",
                innerException)
        { }
    }
}
