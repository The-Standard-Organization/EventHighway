// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class EventArchiveV1OrchestrationServiceException : Xeption
    {
        public FailedEventAddressV1StorageException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
