// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    public class NullEventParticipantSecretV2QueryException : Xeption
    {
        public NullEventParticipantSecretV2QueryException(string message)
            : base(message)
        { }
    }
}
