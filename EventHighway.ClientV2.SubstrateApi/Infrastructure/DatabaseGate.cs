// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApi.Infrastructure
{
    /// <summary>
    /// One-at-a-time access to the app's two EF contexts (the EventHighway substrate and the media
    /// catalogue). A console sample never needs this — it does one thing at a time — but a web host
    /// serves requests in parallel, and both contexts are single, shared, and not thread-safe.
    /// Without a gate, two submissions landing together produce the familiar "a second operation was
    /// started on this context instance" failure.
    /// </summary>
    /// <remarks>
    /// The gate is re-entrant, and it has to be: submitting an event dispatches it in-process, and
    /// the listening service publishes an event of its own from inside that dispatch — the same
    /// request, re-entering the substrate one level down. A plain semaphore would have that call
    /// wait for a lock its own caller is holding, and the request would hang forever.
    ///
    /// Re-entry is recognised by execution flow rather than by thread, because the chain is a chain
    /// of awaits: an <see cref="AsyncLocal{T}"/> flag set before an operation runs is visible to
    /// everything that operation goes on to await, and to nothing else. A second, unrelated request
    /// carries no flag and waits its turn, which is the whole point.
    ///
    /// Nothing on the /receive path takes this gate. That matters: a delivery arrives over HTTP
    /// while the submitting request is still inside the dispatch that produced it, so a /receive
    /// that waited here would deadlock against the very request it is answering.
    /// </remarks>
    public sealed class DatabaseGate
    {
        private static readonly AsyncLocal<bool> IsHeldByCurrentFlow = new();

        private readonly SemaphoreSlim gate = new(initialCount: 1, maxCount: 1);

        public async ValueTask<T> ExecuteAsync<T>(
            Func<ValueTask<T>> operation,
            CancellationToken cancellationToken = default)
        {
            if (IsHeldByCurrentFlow.Value)
            {
                return await operation().ConfigureAwait(false);
            }

            await this.gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            IsHeldByCurrentFlow.Value = true;

            try
            {
                return await operation().ConfigureAwait(false);
            }
            finally
            {
                IsHeldByCurrentFlow.Value = false;
                this.gate.Release();
            }
        }

        public async ValueTask ExecuteAsync(
            Func<ValueTask> operation,
            CancellationToken cancellationToken = default)
        {
            await ExecuteAsync<object>(
                async () =>
                {
                    await operation().ConfigureAwait(false);

                    return null;
                },
                cancellationToken);
        }
    }
}
