// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventV1ArchiveProcessingServiceException : Xeption
    {
        public FailedListenerEventV1ArchiveProcessingServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
