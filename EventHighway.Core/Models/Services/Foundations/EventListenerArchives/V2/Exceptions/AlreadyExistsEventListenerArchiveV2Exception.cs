// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions
{
    public class AlreadyExistsEventListenerArchiveV2Exception : Xeption
    {
        public AlreadyExistsEventListenerArchiveV2Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
