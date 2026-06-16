// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.Core.Clients.EventListeners.V2
{
    /// <summary>
    /// Defines the contract for the V2 event listener client, providing operations to register,
    /// retrieve, and remove event listeners.
    /// </summary>
    public interface IEventListenerV2Client
    {
        /// <summary>
        /// Registers a new event listener asynchronously.
        /// </summary>
        /// <param name="eventListenerV2">The event listener to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventListenerV2}"/> representing the asynchronous
        /// operation that returns the registered event listener.</returns>
        /// <exception cref="ArgumentNullException">Thrown when eventListenerV2 is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventListenerV2> RegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves event listeners by event address identifier asynchronously.
        /// </summary>
        /// <param name="eventAddressId">The identifier of the event address to filter
        /// listeners by.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of event listeners for the specified
        /// event address.</returns>
        /// <exception cref="ArgumentException">Thrown when eventAddressId is an empty
        /// Guid.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an event listener by its identifier asynchronously.
        /// </summary>
        /// <param name="eventListenerV2Id">The identifier of the event listener to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventListenerV2}"/> representing the asynchronous
        /// operation that returns the removed event listener.</returns>
        /// <exception cref="ArgumentException">Thrown when eventListenerV2Id is an empty
        /// Guid.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default);
    }
}
