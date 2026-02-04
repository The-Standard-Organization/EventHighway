// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions
{
    public class NullEventV1CoordinationException : Xeption
    {
        public NullEventV1CoordinationException(string message)
            : base(message)
        { }
    }
}
