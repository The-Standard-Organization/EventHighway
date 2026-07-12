// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;

namespace EventHighway.Core.Clients.EventHandlers.V2
{
    /// <summary>
    /// Defines the contract for the V2 event handler client, providing event handler
    /// registration, retrieval-or-registration, and removal operations.
    /// </summary>
    public interface IEventHandlerV2Client
    {
        /// <summary>
        /// Registers an event handler asynchronously, persisting it to storage and adding it
        /// to the in-memory event handler registry.
        /// </summary>
        /// <param name="eventHandler">The event handler to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IEventHandler}"/> representing the asynchronous
        /// operation that returns the registered event handler.</returns>
        ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an existing event handler by its id or registers the provided one
        /// asynchronously if it does not exist yet.
        /// </summary>
        /// <param name="eventHandler">The event handler to retrieve or register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IEventHandler}"/> representing the asynchronous
        /// operation that returns the retrieved or registered event handler.</returns>
        ValueTask<IEventHandler> RetrieveOrRegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all known event handlers asynchronously — the handlers registered in the
        /// current process when any exist, otherwise the persisted event handler registrations.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of event handlers.</returns>
        ValueTask<IQueryable<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an event handler by its identifier asynchronously, deleting it from
        /// storage and removing it from the in-memory event handler registry.
        /// </summary>
        /// <param name="eventHandlerV2Id">The identifier of the event handler to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventHandlerV2}"/> representing the asynchronous
        /// operation that returns the removed event handler.</returns>
        ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default);
    }
}
