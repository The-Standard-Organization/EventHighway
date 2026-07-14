// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Apis
{
    public interface IApiBroker
    {
        /// <summary>
        /// POSTs a media item to this app's own /submit endpoint, presenting the participant
        /// credentials in the request headers — exactly as an outside contributor would.
        /// </summary>
        ValueTask<HttpResponseMessage> PostMediaItemAsync(
            string url,
            string participantId,
            string participantSecret,
            string content,
            CancellationToken cancellationToken = default);
    }
}
