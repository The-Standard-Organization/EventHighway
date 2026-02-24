// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class NullEventV1ArchiveException : Xeption
    {
        public NullEventV1ArchiveException(string message)
            : base(message)
        { }
    }
}
