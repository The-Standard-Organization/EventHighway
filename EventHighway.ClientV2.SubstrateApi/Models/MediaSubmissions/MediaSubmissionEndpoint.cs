// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions
{
    /// <summary>
    /// The call the send button makes, described rather than made: the verb, the url, and every
    /// header that goes with it. The chat shows this so the same request can be reproduced by hand
    /// in Postman — the credentials on screen are the ones this app is actually configured with,
    /// not an example that drifts out of date.
    /// </summary>
    public sealed class MediaSubmissionEndpoint
    {
        public string Method { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public List<MediaSubmissionHeader> Headers { get; init; } = new();
    }
}
