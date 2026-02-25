// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class FailedEventV1ArchiveProcessingServiceException : Xeption
    {
        public FailedEventV1ArchiveProcessingServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
