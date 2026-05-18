using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class InvalidEventArchiveV1ReferenceException : Xeption
    {
        public InvalidEventArchiveV1ReferenceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
