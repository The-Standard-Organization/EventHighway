// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class InvalidEventV1ArchiveException : Xeption
    {
        public InvalidEventV1ArchiveException(string message)
            : base(message)
        { }
    }
}
