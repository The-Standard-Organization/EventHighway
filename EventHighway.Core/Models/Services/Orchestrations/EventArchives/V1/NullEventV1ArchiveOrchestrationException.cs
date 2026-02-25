// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public partial class NullEventV1ArchiveOrchestrationException : Xeption
    {
        public NullEventV1ArchiveOrchestrationException(string message)
            : base(message)
        { }
    }
}
