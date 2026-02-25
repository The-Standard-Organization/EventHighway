// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class EventV1ArchiveProcessingServiceException : Xeption
    {
        public EventV1ArchiveProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
