// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets.Exceptions
{
    public class FailedEventParticipantSecretsViewServiceException : Xeption
    {
        public FailedEventParticipantSecretsViewServiceException(Exception innerException)
            : base(
                message: "Failed event participant secrets view service error occurred, " +
                    "contact support.",
                innerException)
        { }
    }
}
