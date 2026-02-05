// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventV1ArchiveStorageException : Xeption
    {
        public FailedListenerEventV1ArchiveStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
