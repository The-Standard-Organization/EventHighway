// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class EventArchiveV1ProcessingServiceException : Xeption
    {
        public EventArchiveV1ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
