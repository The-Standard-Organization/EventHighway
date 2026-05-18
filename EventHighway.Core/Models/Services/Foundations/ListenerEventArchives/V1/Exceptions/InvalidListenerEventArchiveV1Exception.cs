// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions
{
    public class InvalidListenerEventArchiveV1Exception : Xeption
    {
        public InvalidListenerEventArchiveV1Exception(string message)
            : base(message)
        { }
    }
}
