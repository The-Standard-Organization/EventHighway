// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.UserEventParticipants.Exceptions
{
    public class UserEventParticipantsViewServiceException : Xeption
    {
        public UserEventParticipantsViewServiceException(Xeption innerException)
            : base(
                message: "User event participant view service error occurred, contact support.",
                innerException)
        { }
    }
}
