// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class NullListenerEventV1ArchivesOrchestrationException : Xeption
    {
        public NullListenerEventV1ArchivesOrchestrationException(string message)
            : base(message)
        { }
    }
}
