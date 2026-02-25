// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class EventV1ArchiveProcessingValidationException : Xeption
    {
        public EventV1ArchiveProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
