// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions
{
    public class UsersViewDependencyException : Xeption
    {
        public UsersViewDependencyException(Xeption innerException)
            : base(
                message: "Users view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
