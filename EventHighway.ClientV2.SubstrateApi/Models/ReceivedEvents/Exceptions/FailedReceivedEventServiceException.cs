// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents.Exceptions
{
    public class FailedReceivedEventServiceException : Xeption
    {
        public FailedReceivedEventServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
