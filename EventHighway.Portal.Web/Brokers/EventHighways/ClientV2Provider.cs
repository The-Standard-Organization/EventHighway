// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Clients.EventHighways.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    /// <summary>
    /// Provides thread-safe lazy initialization of an <see cref="IClientV2"/> instance.
    /// The client is constructed once on first access using double-checked locking.
    /// Failed construction attempts are not cached, allowing automatic recovery
    /// when transient issues (e.g., database connectivity) are resolved.
    /// </summary>
    public sealed class ClientV2Provider
    {
        private readonly Func<IClientV2> clientFactory;
        private readonly object gate = new();
        private IClientV2? client;

        public ClientV2Provider(Func<IClientV2> clientFactory) =>
            this.clientFactory = clientFactory;

        /// <summary>
        /// Gets the shared <see cref="IClientV2"/> instance, initializing it on first access
        /// if necessary. Initialization is thread-safe and occurs at most once.
        /// </summary>
        /// <returns>The initialized client instance.</returns>
        public IClientV2 GetClient()
        {
            IClientV2? current = this.client;

            if (current is not null)
            {
                return current;
            }

            lock (this.gate)
            {
                return this.client ??= this.clientFactory();
            }
        }

        /// <summary>
        /// Executes an asynchronous operation using the shared client instance.
        /// </summary>
        /// <typeparam name="T">The return type of the operation.</typeparam>
        /// <param name="operation">The operation to execute with the client.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The result of the operation.</returns>
        public ValueTask<T> ExecuteAsync<T>(
            Func<IClientV2, ValueTask<T>> operation,
            CancellationToken cancellationToken = default) =>
            operation(GetClient());

        /// <summary>
        /// Executes an asynchronous operation using the shared client instance.
        /// </summary>
        /// <param name="operation">The operation to execute with the client.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public ValueTask ExecuteAsync(
            Func<IClientV2, ValueTask> operation,
            CancellationToken cancellationToken = default) =>
            operation(GetClient());
    }
}
