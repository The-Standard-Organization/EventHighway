// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards.Exceptions
{
    public class HealthViewDependencyException : Xeption
    {
        public HealthViewDependencyException(Xeption innerException)
            : base(
                message: "Health view dependency error occurred, " +
                    "contact support.",
                innerException)
        { }
    }
}
