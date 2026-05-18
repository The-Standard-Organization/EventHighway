// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class FailedEventArchiveV1ProcessingServiceException : Xeption
    {
        public FailedEventArchiveV1ProcessingServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
