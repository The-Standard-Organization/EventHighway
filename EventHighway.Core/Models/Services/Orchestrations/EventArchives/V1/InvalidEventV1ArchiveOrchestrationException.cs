// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public partial class InvalidEventV1ArchiveOrchestrationException : Xeption
    {
        public InvalidEventV1ArchiveOrchestrationException(string message)
            : base(message)
        { }
    }
}
