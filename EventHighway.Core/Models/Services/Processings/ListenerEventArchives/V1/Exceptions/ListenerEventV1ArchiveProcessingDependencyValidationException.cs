// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class ListenerEventV1ArchiveProcessingDependencyValidationException : Xeption
    {
        public ListenerEventV1ArchiveProcessingDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
