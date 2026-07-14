// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Apis
{
    public sealed class ApiBroker : IApiBroker
    {
        // The names are the broker's — it is the thing that sets them on the wire — and they are
        // public because the chat publishes the call it makes, so a reader can reproduce it in
        // Postman. The value of each one is passed in: the broker knows the shape of the request,
        // never whose credentials go into it.
        public const string ParticipantHeader = "X-EventHighwayParticipant";
        public const string ParticipantSecretHeader = "X-EventHighwayParticipantSecret";
        public const string ContentType = "application/json";

        private readonly HttpClient httpClient;

        public ApiBroker(HttpClient httpClient) =>
            this.httpClient = httpClient;

        public async ValueTask<HttpResponseMessage> PostMediaItemAsync(
            string url,
            string participantId,
            string participantSecret,
            string content,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    content,
                    encoding: Encoding.UTF8,
                    mediaType: ContentType)
            };

            request.Headers.Add(ParticipantHeader, participantId);
            request.Headers.Add(ParticipantSecretHeader, participantSecret);

            return await this.httpClient.SendAsync(request, cancellationToken);
        }
    }
}
