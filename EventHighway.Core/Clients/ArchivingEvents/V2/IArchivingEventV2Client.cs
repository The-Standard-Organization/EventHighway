// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    /// <summary>
    /// Defines the contract for the V2 archiving event client, providing operations to archive
    /// dead events.
    /// </summary>
    public interface IArchivingEventV2Client
    {
        /// <summary>
        /// Archives dead events asynchronously. This operation processes and moves dead events
        /// to an archive location.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask ArchiveEventV2sAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Purges archived events older than the specified date asynchronously. This operation
        /// removes archived events that are older than the provided threshold date.
        /// </summary>
        /// <param name="olderThan">The date threshold. Events archived before this date will be
        /// purged.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask PurgeEventArchiveV2sAsync(DateTimeOffset olderThan, CancellationToken cancellationToken = default);

        /// <summary>
        /// Purges archived events older than the configured retention window asynchronously. The
        /// retention window is read from <c>EventHighwayConfiguration.Purging.RetentionDays</c>.
        /// This is the entry point a scheduler uses to purge archive rows on a cadence, whereas the
        /// <see cref="PurgeEventArchiveV2sAsync(DateTimeOffset, CancellationToken)"/> overload
        /// supports a manual purge to an explicit threshold.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask PurgeEventArchiveV2sAsync(CancellationToken cancellationToken = default);
    }
}
