// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Core.Models.Services.Foundations.EventCalls.V2
{
    internal class EventCallV2
    {
        public string HandlerName { get; set; }
        public Dictionary<string, string> HandlerParams { get; set; } = new();
        public string Content { get; set; }
        public string Response { get; set; }
    }
}
