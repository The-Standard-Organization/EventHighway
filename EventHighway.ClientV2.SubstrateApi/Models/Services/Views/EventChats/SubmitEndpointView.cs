// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats
{
    public sealed class SubmitEndpointView
    {
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public List<SubmitHeaderView> Headers { get; set; } = new();

        /// <summary>
        /// A sample body to go with the call, so the whole request can be lifted off the screen
        /// rather than half of it.
        /// </summary>
        public string SampleBody { get; set; } = string.Empty;

        /// <summary>
        /// The same request as a single cURL command, quoted so that it runs as pasted — in a
        /// Command Prompt, in bash, or imported straight into Postman.
        /// </summary>
        public string CurlCommand { get; set; } = string.Empty;
    }
}
