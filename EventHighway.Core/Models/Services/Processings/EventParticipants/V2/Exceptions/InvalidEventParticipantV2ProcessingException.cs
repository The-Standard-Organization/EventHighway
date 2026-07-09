// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions
{
    public class InvalidEventParticipantV2ProcessingException : Xeption
    {
        public InvalidEventParticipantV2ProcessingException(string message)
            : base(message)
        { }
    }
}
