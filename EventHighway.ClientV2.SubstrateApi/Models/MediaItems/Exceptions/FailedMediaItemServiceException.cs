// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaItems.Exceptions
{
    public class FailedMediaItemServiceException : Xeption
    {
        public FailedMediaItemServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
