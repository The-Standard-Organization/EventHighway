// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers.Exceptions
{
    public class FailedEventHandlersViewServiceException : Xeption
    {
        public FailedEventHandlersViewServiceException(Exception innerException)
            : base(
                message: "Failed event handlers view service error occurred, " +
                    "contact support.",
                innerException)
        { }
    }
}
