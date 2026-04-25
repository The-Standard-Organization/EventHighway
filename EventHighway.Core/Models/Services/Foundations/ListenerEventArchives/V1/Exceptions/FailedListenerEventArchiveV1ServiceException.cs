// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class FailedListenerEventArchiveV1ServiceException : Xeption
    {
        public FailedListenerEventArchiveV1ServiceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
