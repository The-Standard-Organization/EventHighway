// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventArchiveV1ServiceException : Xeption
    {
        public FailedListenerEventArchiveV1ServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
