// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class InvalidListenerEventV1ArchiveReferenceException : Xeption
    {
        public InvalidListenerEventV1ArchiveReferenceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
