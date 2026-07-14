// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions
{
    /// <summary>
    /// What the /submit endpoint answered when this app's own UI posted a media item to it. The
    /// UI goes over HTTP on purpose: the send button exercises the very same public intake that
    /// Postman does, rather than a private shortcut into the services behind it.
    /// </summary>
    public sealed class MediaSubmission
    {
        public bool IsAccepted { get; init; }
        public string ResponseCode { get; init; } = string.Empty;
        public string Response { get; init; } = string.Empty;
    }
}
