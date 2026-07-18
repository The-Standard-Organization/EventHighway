// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    public class InvalidEventParticipantSecretV2QueryException : Xeption
    {
        public InvalidEventParticipantSecretV2QueryException(string message)
            : base(message)
        { }
    }
}
