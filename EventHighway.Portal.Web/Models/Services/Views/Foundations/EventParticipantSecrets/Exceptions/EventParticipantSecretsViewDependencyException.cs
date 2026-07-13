// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets.Exceptions
{
    public class EventParticipantSecretsViewDependencyException : Xeption
    {
        public EventParticipantSecretsViewDependencyException(Xeption innerException)
            : base(
                message: "Event participant secrets view dependency error occurred, " +
                    "contact support.",
                innerException)
        { }
    }
}
