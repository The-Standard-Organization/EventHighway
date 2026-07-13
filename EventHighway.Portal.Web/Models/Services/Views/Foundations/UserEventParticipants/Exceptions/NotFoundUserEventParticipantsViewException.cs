// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants.Exceptions
{
    public class NotFoundUserEventParticipantsViewException : Xeption
    {
        public NotFoundUserEventParticipantsViewException()
            : base(
                message: "Could not find the requested user or event participant, " +
                    "verify the details and try again.")
        { }
    }
}
