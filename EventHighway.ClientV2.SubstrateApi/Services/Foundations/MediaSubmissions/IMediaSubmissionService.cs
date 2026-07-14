// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaSubmissions
{
    public interface IMediaSubmissionService
    {
        /// <summary>
        /// Posts a media item, as raw JSON, to this app's own /submit endpoint and reports back
        /// what the endpoint answered.
        /// </summary>
        ValueTask<MediaSubmission> SubmitMediaItemAsync(
            string content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Describes that same call — verb, url and headers, credentials included — without
        /// making it, so a reader can reproduce it in Postman.
        /// </summary>
        ValueTask<MediaSubmissionEndpoint> RetrieveMediaSubmissionEndpointAsync();
    }
}
