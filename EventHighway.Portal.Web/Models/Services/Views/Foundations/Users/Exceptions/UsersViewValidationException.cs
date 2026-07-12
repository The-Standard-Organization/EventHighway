// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions
{
    public class UsersViewValidationException : Xeption
    {
        public UsersViewValidationException(Xeption innerException)
            : base(
                message: "Users view validation error occurred, fix the errors and try again.",
                innerException)
        { }
    }
}
