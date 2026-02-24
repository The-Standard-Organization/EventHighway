// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class AlreadyExistsEventV1ArchiveException : Xeption
    {
        public AlreadyExistsEventV1ArchiveException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
