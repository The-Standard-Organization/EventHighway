// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions
{
    public class InvalidEventArchiveV2OrchestrationException : Xeption
    {
        public InvalidEventArchiveV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
