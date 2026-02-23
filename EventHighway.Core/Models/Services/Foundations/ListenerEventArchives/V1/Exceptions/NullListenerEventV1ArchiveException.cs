// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class NullListenerEventV1ArchiveException : Xeption
    {
        public NullListenerEventV1ArchiveException(string message)
            : base(message)
        { }
    }
}
