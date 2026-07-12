// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions
{
    public class LastAdministratorUsersViewException : Xeption
    {
        public LastAdministratorUsersViewException()
            : base(
                message: "Cannot remove the last administrator; at least one administrator " +
                    "must remain.")
        { }
    }
}
