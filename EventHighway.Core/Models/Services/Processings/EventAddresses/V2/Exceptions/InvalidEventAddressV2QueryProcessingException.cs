// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions
{
    public class InvalidEventAddressV2QueryProcessingException : Xeption
    {
        public InvalidEventAddressV2QueryProcessingException(string message)
            : base(message)
        { }
    }
}
