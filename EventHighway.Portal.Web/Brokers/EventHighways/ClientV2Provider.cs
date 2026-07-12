// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Clients.EventHighways.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    // Builds the EventHighway V2 client once, lazily, and serialized behind a lock so the many
    // dashboard panels that first-touch the broker concurrently never construct it twice (which
    // would race on Database.Migrate() over the same Core database and fail every call).
    //
    // Unlike a cached Lazy<T>, a failed construction is NOT remembered: if the factory throws (e.g.
    // a LocalDB cold-start SqlException) the client stays null and the next request retries, so the
    // portal recovers once the database becomes reachable — no app restart required.
    //
    // The client wraps a SINGLE Core EF DbContext, which is not thread-safe. Blazor Server fans the
    // dashboard panels out concurrently, so every database call is funnelled through ExecuteAsync,
    // which holds a one-at-a-time semaphore for the duration of the operation. This guarantees no two
    // operations ever touch the shared DbContext at the same time (the "A second operation was started
    // on this context instance" / "used while it is being configured" failures).
    public sealed class ClientV2Provider
    {
        private readonly Func<IClientV2> clientFactory;
        private readonly object gate = new();
        private readonly SemaphoreSlim databaseGate = new(initialCount: 1, maxCount: 1);
        private IClientV2? client;

        public ClientV2Provider(Func<IClientV2> clientFactory) =>
            this.clientFactory = clientFactory;

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

        public async ValueTask<T> ExecuteAsync<T>(
            Func<IClientV2, ValueTask<T>> operation,
            CancellationToken cancellationToken = default)
        {
            IClientV2 currentClient = GetClient();

            await this.databaseGate.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                return await operation(currentClient).ConfigureAwait(false);
            }
            finally
            {
                this.databaseGate.Release();
            }
        }

        public async ValueTask ExecuteAsync(
            Func<IClientV2, ValueTask> operation,
            CancellationToken cancellationToken = default)
        {
            IClientV2 currentClient = GetClient();

            await this.databaseGate.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await operation(currentClient).ConfigureAwait(false);
            }
            finally
            {
                this.databaseGate.Release();
            }
        }
    }
}
