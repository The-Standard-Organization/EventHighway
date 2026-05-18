// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventArchiveV1ProcessingServiceException : Xeption
    {
        public FailedListenerEventArchiveV1ProcessingServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
