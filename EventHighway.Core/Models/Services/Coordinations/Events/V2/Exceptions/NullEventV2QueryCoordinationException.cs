// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions
{
    public class NullEventV2QueryCoordinationException : Xeption
    {
        public NullEventV2QueryCoordinationException(string message)
            : base(message)
        { }
    }
}
