// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Apis
{
    internal class ApiBroker : IApiBroker
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async ValueTask<HttpResponseMessage> PostEventAsync(
            string url,
            string secret,
            string content,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    content,
                    encoding: Encoding.UTF8,
                    mediaType: "application/json")
            };

            request.Headers.Add(name: "X-Highway", value: secret);

            return await this.httpClient.SendAsync(request, cancellationToken);
        }
    }
}
