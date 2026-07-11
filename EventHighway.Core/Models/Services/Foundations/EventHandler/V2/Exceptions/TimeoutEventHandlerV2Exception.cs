// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions
{
    public class TimeoutEventHandlerV2Exception : Xeption
    {
        public TimeoutEventHandlerV2Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
