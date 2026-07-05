// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Clients.ListenerEvents.V2
{
    /// <summary>
    /// Defines the contract for the V2 listener event client, providing operations to retrieve
    /// and remove listener events.
    /// </summary>
    public interface IListenerEventV2Client
    {
        /// <summary>
        /// Retrieves all listener events asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of all listener events.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all listener events asynchronously with their associated event listeners by 
        /// delegating to the orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of all listener events with their
        /// associated event listeners.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a listener event by its identifier asynchronously.
        /// </summary>
        /// <param name="listenerEventV2Id">The identifier of the listener event to
        /// remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{ListenerEventV2}"/> representing the asynchronous
        /// operation that returns the removed listener event.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retries all failed listener events asynchronously by sweeping and re-dispatching
        /// them.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous retry
        /// operation.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during the retry sweep.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask RetryFailedListenerEventV2sAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets the retry attempts for a listener event by its identifier asynchronously.
        /// </summary>
        /// <param name="listenerEventV2Id">The identifier of the listener event to reset
        /// retries for.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{ListenerEventV2}"/> representing the asynchronous
        /// operation that returns the listener event with reset retries.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during the reset.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<ListenerEventV2> ResetRetriesForListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets the retry attempts for all listener events belonging to an event listener by
        /// its identifier asynchronously.
        /// </summary>
        /// <param name="eventListenerV2Id">The identifier of the event listener whose listener
        /// events should have their retries reset.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous reset
        /// operation.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during the reset.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default);
    }
}
