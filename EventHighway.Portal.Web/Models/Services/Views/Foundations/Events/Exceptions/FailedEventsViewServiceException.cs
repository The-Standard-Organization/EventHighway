// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Events.Exceptions
{
    public class FailedEventsViewServiceException : Xeption
    {
        public FailedEventsViewServiceException(Exception innerException)
            : base(
                message: "Failed events view service error occurred, contact support.",
                innerException)
        { }
    }
}
