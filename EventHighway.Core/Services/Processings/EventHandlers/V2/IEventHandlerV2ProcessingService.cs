// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal interface IEventHandlerV2ProcessingService
    {
        ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);
    }
}
