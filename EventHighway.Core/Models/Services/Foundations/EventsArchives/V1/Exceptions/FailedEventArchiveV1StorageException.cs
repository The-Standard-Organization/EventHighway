// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class FailedEventArchiveV1StorageException : Xeption
    {
        public FailedEventArchiveV1StorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
