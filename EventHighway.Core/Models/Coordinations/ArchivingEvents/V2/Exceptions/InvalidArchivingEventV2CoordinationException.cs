// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions
{
    internal class InvalidArchivingEventV2CoordinationException : Xeption
    {
        public InvalidArchivingEventV2CoordinationException(string message)
            : base(message)
        { }
    }
}
