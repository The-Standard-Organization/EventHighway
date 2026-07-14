// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents.Exceptions
{
    public class InvalidReceivedEventException : Xeption
    {
        public InvalidReceivedEventException(string message)
            : base(message)
        { }
    }
}
