// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Clients
{
    /// <summary>
    /// A delegate client for Joe's REST API: its exposed method matches the
    /// <c>Func&lt;string, CancellationToken, ValueTask&lt;EventHandlerResult&gt;&gt;</c>
    /// signature that <c>DelegateEventHandler</c> accepts, so it can be passed as a
    /// method group when registering Joe's handler with an EventHighway client.
    /// </summary>
    public interface IJoesRestApiDelegateClient
    {
        /// <summary>
        /// Posts the raw event content to Joe's REST API, authenticated with the
        /// configured <c>X-Highway</c> secret, and reports the delivery outcome.
        /// Never throws — validation problems map to a 400-shaped result, delivery
        /// problems to a 502, and anything else to a 500.
        /// </summary>
        ValueTask<EventHandlerResult> PostToJoesRestApiAsync(
            string content,
            CancellationToken cancellationToken);
    }
}
