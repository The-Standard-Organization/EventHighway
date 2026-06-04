// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions
{
    public class AlreadyExistsEventV1Exception : Xeption
    {
        public AlreadyExistsEventV1Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
