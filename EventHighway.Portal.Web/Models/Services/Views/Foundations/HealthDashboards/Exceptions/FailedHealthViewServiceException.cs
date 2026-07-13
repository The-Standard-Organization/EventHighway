// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards.Exceptions
{
    public class FailedHealthViewServiceException : Xeption
    {
        public FailedHealthViewServiceException(Exception innerException)
            : base(
                message: "Failed health view service error occurred, contact support.",
                innerException)
        { }
    }
}
