// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions
{
    public class InvalidEventV2QueryCoordinationException : Xeption
    {
        public InvalidEventV2QueryCoordinationException(string message)
            : base(message)
        { }
    }
}
