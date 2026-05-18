// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class ListenerEventArchiveV1DependencyValidationException : Xeption
    {
        public ListenerEventArchiveV1DependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
