// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class NullListenerEventArchiveV1sOrchestrationException : Xeption
    {
        public NullListenerEventArchiveV1sOrchestrationException(string message)
            : base(message)
        { }
    }
}
