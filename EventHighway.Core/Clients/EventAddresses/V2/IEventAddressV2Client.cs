// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Core.Clients.EventAddresses.V2
{
    /// <summary>
    /// Defines the contract for the V2 event address client, providing operations to register,
    /// retrieve, and remove event addresses.
    /// </summary>
    public interface IEventAddressV2Client
    {
        /// <summary>
        /// Registers a new event address asynchronously.
        /// </summary>
        /// <param name="eventAddressV2">The event address to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the registered event address.</returns>
        /// <exception cref="ArgumentNullException">Thrown when eventAddressV2 is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an existing event address or registers a new one asynchronously.
        /// </summary>
        ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all event addresses asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of event addresses.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an event address by its identifier asynchronously.
        /// </summary>
        /// <param name="eventAddressV2Id">The identifier of the event address to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the removed event address.</returns>
        /// <exception cref="ArgumentException">Thrown when eventAddressV2Id is an empty
        /// Guid.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default);
    }
}
