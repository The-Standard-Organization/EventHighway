// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions
{
    internal class EventHandlerV2DependencyException : Xeption
    {
        public EventHandlerV2DependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
