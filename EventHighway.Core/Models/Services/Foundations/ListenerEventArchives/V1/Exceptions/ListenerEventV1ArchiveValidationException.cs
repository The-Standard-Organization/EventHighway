// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class ListenerEventV1ArchiveValidationException : Xeption
    {
        public ListenerEventV1ArchiveValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
