// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions
{
    internal class EventListenerArchiveV2DependencyValidationException : Xeption
    {
        public EventListenerArchiveV2DependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
