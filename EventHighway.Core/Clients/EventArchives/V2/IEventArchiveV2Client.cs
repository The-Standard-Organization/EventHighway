// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Core.Clients.EventArchives.V2
{
    /// <summary>
    /// Defines the contract for the V2 event archive client, providing read operations over
    /// archived events.
    /// </summary>
    public interface IEventArchiveV2Client
    {
        /// <summary>
        /// Retrieves all archived events asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous operation
        /// that returns all archived events.</returns>
        /// <exception cref="EventArchiveV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventArchiveV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventArchiveV2ClientServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all archived events asynchronously with their associated event address V2.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous operation
        /// that returns all archived events with their associated event address V2.</returns>
        /// <exception cref="EventArchiveV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventArchiveV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventArchiveV2ClientServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sWithEventAddressV2Async(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an archived event by its identifier asynchronously.
        /// </summary>
        /// <param name="eventArchiveV2Id">The identifier of the archived event to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventArchiveV2}"/> representing the asynchronous
        /// operation that returns the retrieved archived event.</returns>
        /// <exception cref="EventArchiveV2ClientValidationException">Thrown when validation errors
        /// occur.</exception>
        /// <exception cref="EventArchiveV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventArchiveV2ClientServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default);
    }
}
