// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;

namespace EventHighway.Core.Clients.EventParticipants.V2
{
    /// <summary>
    /// Defines the contract for the V2 event participant client, providing registration,
    /// retrieval, modification, and removal operations over event participants.
    /// </summary>
    public interface IEventParticipantV2Client
    {
        /// <summary>
        /// Adds a new event participant asynchronously.
        /// </summary>
        /// <param name="eventParticipantV2">The event participant to add.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantV2}"/> representing the asynchronous
        /// operation that returns the added event participant.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an existing event participant by its id or adds the provided one
        /// asynchronously if it does not exist yet.
        /// </summary>
        /// <param name="eventParticipantV2">The event participant to retrieve or add.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantV2}"/> representing the asynchronous
        /// operation that returns the existing or newly added event participant.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantV2> RetrieveOrAddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the event participants matching the given query asynchronously — filtered,
        /// ordered by <c>CreatedDate</c> descending, paged, and materialized at the time of the
        /// call.
        /// </summary>
        /// <param name="eventParticipantV2Query">The search criteria; omitted criteria are not
        /// applied.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IReadOnlyList}"/> representing the asynchronous
        /// operation that returns the matching page of event participants.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            EventParticipantV2Query eventParticipantV2Query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an event participant by its identifier asynchronously.
        /// </summary>
        /// <param name="eventParticipantV2Id">The identifier of the event participant to
        /// retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantV2}"/> representing the asynchronous
        /// operation that returns the retrieved event participant.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies an existing event participant asynchronously.
        /// </summary>
        /// <param name="eventParticipantV2">The event participant carrying the updated
        /// values.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantV2}"/> representing the asynchronous
        /// operation that returns the modified event participant.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an event participant by its identifier asynchronously.
        /// </summary>
        /// <param name="eventParticipantV2Id">The identifier of the event participant to
        /// remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventParticipantV2}"/> representing the asynchronous
        /// operation that returns the removed event participant.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventParticipantV2> RemoveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default);
    }
}
