using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class InvalidEventV1ArchiveReferenceException : Xeption
    {
        public InvalidEventV1ArchiveReferenceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
