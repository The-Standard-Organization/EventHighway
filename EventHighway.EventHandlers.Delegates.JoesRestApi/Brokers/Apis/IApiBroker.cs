// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Apis
{
    internal interface IApiBroker
    {
        ValueTask<HttpResponseMessage> PostEventAsync(
            string url,
            string secret,
            string content,
            CancellationToken cancellationToken);
    }
}
