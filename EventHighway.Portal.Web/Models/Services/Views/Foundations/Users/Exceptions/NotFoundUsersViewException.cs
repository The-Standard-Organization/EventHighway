// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions
{
    public class NotFoundUsersViewException : Xeption
    {
        public NotFoundUsersViewException(Guid userId)
            : base(message: $"User with id {userId} was not found.")
        { }
    }
}
