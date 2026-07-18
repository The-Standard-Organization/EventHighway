// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Clients.EventParticipantSecrets.V2
{
    /// <summary>
    /// Defines the contract for the V2 event participant secret client, providing add, retrieval,
    /// modification, and removal operations over event participant secrets.
    /// </summary>
    public interface IEventParticipantSecretV2Client
    {
        /// <summary>
        /// Adds a new event participant secret asynchronously.
        /// </summary>
        /// <param name="eventParticipantSecretV2">The event participant secret to add.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantSecretV2}"/> representing the
        /// asynchronous operation that returns the added event participant secret.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the event participant secrets matching the given query asynchronously —
        /// filtered, ordered by <c>CreatedDate</c> descending, paged, and materialized at the
        /// time of the call.
        /// </summary>
        /// <param name="eventParticipantSecretV2Query">The search criteria; omitted criteria are
        /// not applied.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IReadOnlyList}"/> representing the asynchronous
        /// operation that returns the matching page of event participant secrets.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<EventParticipantSecretV2>> RetrieveAllEventParticipantSecretV2sAsync(
            EventParticipantSecretV2Query eventParticipantSecretV2Query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an event participant secret by its identifier asynchronously.
        /// </summary>
        /// <param name="eventParticipantSecretV2Id">The identifier of the event participant secret
        /// to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantSecretV2}"/> representing the
        /// asynchronous operation that returns the retrieved event participant secret.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantSecretV2> RetrieveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies an existing event participant secret asynchronously.
        /// </summary>
        /// <param name="eventParticipantSecretV2">The event participant secret carrying the
        /// updated values.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantSecretV2}"/> representing the
        /// asynchronous operation that returns the modified event participant secret.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an event participant secret by its identifier asynchronously.
        /// </summary>
        /// <param name="eventParticipantSecretV2Id">The identifier of the event participant secret
        /// to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantSecretV2}"/> representing the
        /// asynchronous operation that returns the removed event participant secret.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantSecretV2> RemoveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default);
    }
}
