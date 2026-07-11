// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Services.Processings.EventHandlers.V2;

namespace EventHighway.Core.Clients.EventHandlers.V2
{
    /// <summary>
    /// Represents the V2 event handler client implementation, handling event handler
    /// registration, retrieval-or-registration, and removal operations while managing
    /// processing service exceptions.
    /// </summary>
    internal class EventHandlerV2Client : IEventHandlerV2Client
    {
        private readonly IEventHandlerV2ProcessingService eventHandlerV2ProcessingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerV2Client"/> class with
        /// the specified event handler processing service.
        /// </summary>
        /// <param name="eventHandlerV2ProcessingService">The processing service for managing
        /// event handlers.</param>
        public EventHandlerV2Client(IEventHandlerV2ProcessingService eventHandlerV2ProcessingService) =>
            this.eventHandlerV2ProcessingService = eventHandlerV2ProcessingService;

        public async ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
            await this.eventHandlerV2ProcessingService.RegisterEventHandlerV2Async(
                eventHandler, cancellationToken);

        public ValueTask<IEventHandler> RetrieveOrRegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
