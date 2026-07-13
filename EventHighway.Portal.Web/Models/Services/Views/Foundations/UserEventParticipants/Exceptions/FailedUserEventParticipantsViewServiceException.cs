// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants.Exceptions
{
    public class FailedUserEventParticipantsViewServiceException : Xeption
    {
        public FailedUserEventParticipantsViewServiceException(Exception innerException)
            : base(
                message: "Failed user event participant view service error occurred, " +
                    "contact support.",
                innerException)
        { }
    }
}
