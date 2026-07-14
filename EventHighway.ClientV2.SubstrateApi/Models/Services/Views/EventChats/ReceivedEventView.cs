// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats
{
    public sealed class ReceivedEventView
    {
        public Guid Id { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
