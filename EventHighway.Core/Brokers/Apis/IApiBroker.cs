// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;

namespace EventHighway.Core.Brokers.Apis
{
    internal interface IApiBroker
    {
        ValueTask<string> PostAsync(
            string content,
            string url,
            string secret);

        ValueTask<HttpResponseMessage> PostAsyncV1(
            string content,
            string url,
            string secret);
    }
}
