// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions
{
    public class ListenerEventArchiveV1ProcessingDependencyValidationException : Xeption
    {
        public ListenerEventArchiveV1ProcessingDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
