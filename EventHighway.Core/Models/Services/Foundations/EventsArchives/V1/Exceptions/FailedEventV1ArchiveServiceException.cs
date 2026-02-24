// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class FailedEventV1ArchiveServiceException : Xeption
    {
        public FailedEventV1ArchiveServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
