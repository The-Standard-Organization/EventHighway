// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Services.Foundations.EventPosts
{
    internal interface IEventPostService
    {
        ValueTask<EventHandlerResult> PostEventAsync(
            string content,
            CancellationToken cancellationToken = default);
    }
}
