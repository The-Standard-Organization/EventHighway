// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class EventV1ArchiveDependencyException : Xeption
    {
        public EventV1ArchiveDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
