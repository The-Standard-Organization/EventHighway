// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class InvalidEventV1ArchiveProcessingException : Xeption
    {
        public InvalidEventV1ArchiveProcessingException(string message)
            : base(message)
        { }
    }
}
