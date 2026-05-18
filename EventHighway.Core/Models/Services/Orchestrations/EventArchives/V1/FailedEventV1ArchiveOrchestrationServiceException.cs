// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class FailedEventArchiveV1OrchestrationServiceException : Xeption
    {
        public FailedEventArchiveV1OrchestrationServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
