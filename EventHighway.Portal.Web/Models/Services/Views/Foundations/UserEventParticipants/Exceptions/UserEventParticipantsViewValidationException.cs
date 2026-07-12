// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants.Exceptions
{
    public class UserEventParticipantsViewValidationException : Xeption
    {
        public UserEventParticipantsViewValidationException(Xeption innerException)
            : base(
                message: "User event participant view validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
