// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions
{
    public class EventArchiveV1ProcessingDependencyException : Xeption
    {
        public EventArchiveV1ProcessingDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
