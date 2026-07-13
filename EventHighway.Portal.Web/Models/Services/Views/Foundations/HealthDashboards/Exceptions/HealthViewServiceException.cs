// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards.Exceptions
{
    public class HealthViewServiceException : Xeption
    {
        public HealthViewServiceException(Xeption innerException)
            : base(
                message: "Health view service error occurred, contact support.",
                innerException)
        { }
    }
}
