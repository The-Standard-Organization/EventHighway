// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions
{
    public sealed class MediaSubmissionHeader
    {
        public string Name { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;

        /// <summary>
        /// True for the headers that carry credentials, so the UI can mark them as the two a
        /// caller has to get right.
        /// </summary>
        public bool IsCredential { get; init; }
    }
}
