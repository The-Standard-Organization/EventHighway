// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class ListenerEventV1ArchiveProcessingServiceException : Xeption
    {
        public ListenerEventV1ArchiveProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
