// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants.Exceptions
{
    public class FailedEventParticipantsViewServiceException : Xeption
    {
        public FailedEventParticipantsViewServiceException(Exception innerException)
            : base(
                message: "Failed event participants view service error occurred, " +
                    "contact support.",
                innerException)
        { }
    }
}
