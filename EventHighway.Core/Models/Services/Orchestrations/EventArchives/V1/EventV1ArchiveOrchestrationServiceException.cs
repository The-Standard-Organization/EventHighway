// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class EventArchiveV1OrchestrationServiceException : Xeption
    {
        public EventArchiveV1OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
