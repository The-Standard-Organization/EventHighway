// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.UserEventParticipants.Exceptions
{
    public class AlreadyExistsUserEventParticipantsViewException : Xeption
    {
        public AlreadyExistsUserEventParticipantsViewException()
            : base(
                message: "This user is already associated with that event participant.")
        { }
    }
}
