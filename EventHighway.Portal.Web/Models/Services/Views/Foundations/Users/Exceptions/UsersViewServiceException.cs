// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions
{
    public class UsersViewServiceException : Xeption
    {
        public UsersViewServiceException(Xeption innerException)
            : base(
                message: "Users view service error occurred, contact support.",
                innerException)
        { }
    }
}
