// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal interface IEventHandlerV2ProcessingService
    {
        ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);

        ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<IEventHandler> RetrieveOrRegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventHandlerV2>> RetrieveEventHandlerV2sByQueryAsync(
            EventHandlerV2Query eventHandlerV2Query,
            CancellationToken cancellationToken = default);
    }
}
