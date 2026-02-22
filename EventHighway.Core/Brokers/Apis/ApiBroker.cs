// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventHighway.Core.Brokers.Apis
{
    internal class ApiBroker : IApiBroker
    {
        public async ValueTask<string> PostAsync(string content, string url, string secret)
        {
            var httpClient = new HttpClient();

            var stringContent =
               new StringContent(
                   content,
                   encoding: Encoding.UTF8,
                   mediaType: "application/json");

            httpClient.DefaultRequestHeaders.Add(
                name: "X-Highway",
                value: secret);

            HttpResponseMessage httpResponseMessage =
                await httpClient.PostAsync(
                    requestUri: url,
                    content: stringContent);

            return httpResponseMessage.ToString();
        }
    }
}