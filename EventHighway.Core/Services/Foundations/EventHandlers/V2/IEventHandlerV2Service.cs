// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal interface IEventHandlerV2Service
    {
        ValueTask<IEventHandler> AddEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<IEventHandler>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventHandlerV2>> RetrieveAllEventHandlerV2sFromStorageAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IEventHandler> RetrieveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default);
    }
}
