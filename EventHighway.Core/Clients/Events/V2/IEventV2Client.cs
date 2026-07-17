// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Clients.Events.V2
{
    /// <summary>
    /// Defines the contract for the V2 event client, providing operations to submit, fire, and
    /// remove events.
    /// </summary>
    public interface IEventV2Client
    {
        /// <summary>
        /// Submits an event asynchronously.
        /// </summary>
        /// <param name="eventV2">The event to submit.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventV2}"/> representing the asynchronous operation
        /// that returns the submitted event.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during submission.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires scheduled pending events asynchronously. This operation processes all pending
        /// events that are scheduled to be fired.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during firing.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the events matching the given query asynchronously — filtered, ordered by
        /// <c>CreatedDate</c> descending, paged, and materialized at the time of the call.
        /// </summary>
        /// <param name="eventV2Query">The search criteria; omitted criteria are not
        /// applied.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IReadOnlyList}"/> representing the asynchronous
        /// operation that returns the matching events.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when the query is null or
        /// carries invalid values.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an event by its identifier asynchronously.
        /// </summary>
        /// <param name="eventV2Id">The identifier of the event to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventV2}"/> representing the asynchronous operation
        /// that returns the retrieved event.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an event by its identifier asynchronously.
        /// </summary>
        /// <param name="eventV2Id">The identifier of the event to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventV2}"/> representing the asynchronous operation
        /// that returns the removed event.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);
    }
}
