// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.ExternalMediaItems.Exceptions
{
    public class FailedExternalMediaItemServiceException : Xeption
    {
        public FailedExternalMediaItemServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
