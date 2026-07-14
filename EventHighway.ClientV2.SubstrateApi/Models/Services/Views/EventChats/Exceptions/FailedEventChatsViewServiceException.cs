// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats.Exceptions
{
    public class FailedEventChatsViewServiceException : Xeption
    {
        public FailedEventChatsViewServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
