// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets.Exceptions
{
    public class EventParticipantSecretsViewServiceException : Xeption
    {
        public EventParticipantSecretsViewServiceException(Xeption innerException)
            : base(
                message: "Event participant secrets view service error occurred, contact support.",
                innerException)
        { }
    }
}
