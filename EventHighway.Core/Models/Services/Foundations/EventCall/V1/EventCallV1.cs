// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Services.Foundations.EventCall.V1
{
    internal class EventCallV1
    {
        public string Endpoint { get; set; }
        public string Secret { get; set; }
        public string Content { get; set; }
        public string Response { get; set; }
        public string ResponseReasonPhrase { get; set; }
    }
}
