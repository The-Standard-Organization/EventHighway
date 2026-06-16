// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions
{
    public class NullEventAddressV2ProcessingException : Xeption
    {
        public NullEventAddressV2ProcessingException(string message)
            : base(message)
        { }
    }
}
