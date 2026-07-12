// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets.Exceptions
{
    public class EventParticipantSecretsViewDependencyValidationException : Xeption
    {
        public EventParticipantSecretsViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Event participant secrets view dependency validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
