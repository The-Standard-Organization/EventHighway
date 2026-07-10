// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Foundations.EventPosts.Exceptions
{
    public class FailedEventPostServiceException : Xeption
    {
        public FailedEventPostServiceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
