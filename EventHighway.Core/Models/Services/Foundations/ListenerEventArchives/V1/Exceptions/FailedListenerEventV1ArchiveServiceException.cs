// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventV1ArchiveServiceException : Xeption
    {
        public FailedListenerEventV1ArchiveServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
