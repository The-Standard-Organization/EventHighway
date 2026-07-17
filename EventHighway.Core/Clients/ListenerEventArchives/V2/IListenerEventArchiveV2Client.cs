// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Clients.ListenerEventArchives.V2
{
    /// <summary>
    /// Defines the contract for the V2 listener event archive client, providing read operations
    /// over archived listener events.
    /// </summary>
    public interface IListenerEventArchiveV2Client
    {
        /// <summary>
        /// Retrieves all archived listener events asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous operation
        /// that returns all archived listener events.</returns>
        /// <exception cref="ListenerEventArchiveV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventArchiveV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventArchiveV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all archived listener events asynchronously with their associated event listener V2.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous operation
        /// that returns all archived listener events with their associated event listener V2.</returns>
        /// <exception cref="ListenerEventArchiveV2ClientValidationException">Thrown when validation
        /// errors occur.</exception>
        /// <exception cref="ListenerEventArchiveV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventArchiveV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default);
    }
}
