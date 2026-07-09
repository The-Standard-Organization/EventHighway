// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions
{
    internal class EventParticipantV2ProcessingServiceException : Xeption
    {
        public EventParticipantV2ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
