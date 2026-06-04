// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class EventArchiveV1OrchestrationDependencyException : Xeption
    {
        public FailedEventV1ArchiveOrchestrationServiceException(
            string message,
            Exception innerException, 
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
