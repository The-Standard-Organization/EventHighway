// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class LockedListenerEventV1ArchiveException : Xeption
    {
        public LockedListenerEventV1ArchiveException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
