// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Models.Services.Foundations.EventCall.V2
{
    public class EventCallV2
    {
        public string HandlerName { get; set; }
        public List<HandlerConfiguration> HandlerConfigurations { get; set; } = new();
        public string Content { get; set; }
        public string Response { get; set; }
        public string ResponseReasonPhrase { get; set; }
        public bool IsSuccess { get; set; }
    }
}
