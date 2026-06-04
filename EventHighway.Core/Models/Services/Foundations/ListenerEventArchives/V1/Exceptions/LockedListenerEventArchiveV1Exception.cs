// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class LockedListenerEventArchiveV1Exception : Xeption
    {
        public LockedListenerEventArchiveV1Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
