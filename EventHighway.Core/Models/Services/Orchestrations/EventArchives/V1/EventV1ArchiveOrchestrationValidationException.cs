// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class EventArchiveV1OrchestrationValidationException : Xeption
    {
        public EventArchiveV1OrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
