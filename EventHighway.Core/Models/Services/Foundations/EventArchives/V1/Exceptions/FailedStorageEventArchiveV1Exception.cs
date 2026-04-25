// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventArchives.V1.Exceptions
{
    public class FailedStorageEventArchiveV1Exception : Xeption
    {
        public FailedStorageEventArchiveV1Exception(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
