// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class InvalidListenerEventArchiveV1ReferenceException : Xeption
    {
        public InvalidListenerEventArchiveV1ReferenceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
