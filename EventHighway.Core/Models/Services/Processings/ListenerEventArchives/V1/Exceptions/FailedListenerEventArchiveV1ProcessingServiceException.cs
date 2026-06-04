// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventArchiveV1ProcessingServiceException : Xeption
    {
        public FailedListenerEventArchiveV1ProcessingServiceException(
            string message,
            Exception innerException,
            IDictionary data = null)
            : base(message, innerException, data)
        { }
    }
}
