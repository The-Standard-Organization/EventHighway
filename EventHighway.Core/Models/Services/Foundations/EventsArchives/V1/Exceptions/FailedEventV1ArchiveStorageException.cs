// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class FailedEventV1ArchiveStorageException : Xeption
    {
        public FailedEventV1ArchiveStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
