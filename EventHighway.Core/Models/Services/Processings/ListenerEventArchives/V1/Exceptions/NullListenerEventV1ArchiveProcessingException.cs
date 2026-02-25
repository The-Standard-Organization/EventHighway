// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class NullListenerEventV1ArchiveProcessingException : Xeption
    {
        public NullListenerEventV1ArchiveProcessingException(string message)
            : base(message)
        { }
    }
}
