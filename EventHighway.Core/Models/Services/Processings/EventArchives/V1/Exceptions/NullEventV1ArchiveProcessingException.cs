// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class NullEventV1ArchiveProcessingException : Xeption
    {
        public NullEventV1ArchiveProcessingException(string message)
            : base(message)
        { }
    }
}
