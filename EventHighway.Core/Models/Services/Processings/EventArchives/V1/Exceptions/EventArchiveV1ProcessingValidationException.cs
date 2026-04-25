// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class EventArchiveV1ProcessingValidationException : Xeption
    {
        public EventArchiveV1ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
