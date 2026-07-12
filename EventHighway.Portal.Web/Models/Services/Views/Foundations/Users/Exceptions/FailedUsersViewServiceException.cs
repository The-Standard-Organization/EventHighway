// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions
{
    public class FailedUsersViewServiceException : Xeption
    {
        public FailedUsersViewServiceException(Exception innerException)
            : base(
                message: "Failed users view service error occurred, contact support.",
                innerException)
        { }
    }
}
