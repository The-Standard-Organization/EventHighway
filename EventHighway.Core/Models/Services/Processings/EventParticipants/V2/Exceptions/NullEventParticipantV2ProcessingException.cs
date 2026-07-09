// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions
{
    public class NullEventParticipantV2ProcessingException : Xeption
    {
        public NullEventParticipantV2ProcessingException(string message)
            : base(message)
        { }
    }
}
