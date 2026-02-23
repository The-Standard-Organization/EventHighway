// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class InvalidListenerEventV1ArchiveException : Xeption
    {
        public InvalidListenerEventV1ArchiveException(string message)
            : base(message)
        { }
    }
}
