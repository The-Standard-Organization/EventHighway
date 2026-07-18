// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventArchives.V2;
using EventHighway.Core.Clients.EventHandlers.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.EventParticipantSecrets.V2;
using EventHighway.Core.Clients.EventParticipants.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Clients.ListenerEventArchives.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;
using EventHighway.Core.Clients.ReplayingEvents.V2;

namespace EventHighway.Core.Clients.EventHighways.V2
{
    /// <summary>
    /// Defines the V2 API contract for the EventHighway client, providing access to event
    /// management operations including event archiving, addresses, listeners, events, health
    /// checks, and listener events.
    /// </summary>
    public interface IClientV2
    {
        /// <summary>
        /// Registers an event handler with the EventHighway V2 client, retrieving it if it was
        /// already registered and persisting it to storage otherwise. This method supports
        /// method chaining by returning the current instance.
        /// </summary>
        /// <remarks>
        /// This overload blocks the calling thread on the underlying asynchronous work
        /// (sync-over-async), so it is intended for **startup / composition-root registration
        /// only**. In any asynchronous context (request handlers, background services) prefer
        /// <see cref="RegisterEventHandlerAsync"/> to avoid deadlocks on hosts with a
        /// synchronization context and thread-pool starvation under load.
        /// </remarks>
        /// <param name="eventHandler">The event handler to register.</param>
        /// <returns>The current <see cref="IClientV2"/> instance for method chaining.</returns>
        IClientV2 RegisterEventHandler(IEventHandler eventHandler);

        /// <summary>
        /// Registers an event handler with the EventHighway V2 client asynchronously, retrieving it
        /// if it was already registered and persisting it to storage otherwise. This method supports
        /// asynchronous method chaining by returning the current instance.
        /// </summary>
        /// <param name="eventHandler">The event handler to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IClientV2}"/> representing the asynchronous operation
        /// that returns the current <see cref="IClientV2"/> instance for method chaining.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IClientV2> RegisterEventHandlerAsync(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the client for managing archived events in V2 API.
        /// </summary>
        IArchivingEventV2Client ArchivingEventV2Client { get; }

        /// <summary>
        /// Gets the client for managing event addresses in V2 API.
        /// </summary>
        IEventAddressV2Client EventAddressV2Client { get; }

        /// <summary>
        /// Gets the client for retrieving archived events in V2 API.
        /// </summary>
        IEventArchiveV2Client EventArchiveV2Client { get; }

        /// <summary>
        /// Gets the client for managing event handlers in V2 API.
        /// </summary>
        IEventHandlerV2Client EventHandlerV2Client { get; }

        /// <summary>
        /// Gets the client for managing event listeners in V2 API.
        /// </summary>
        IEventListenerV2Client EventListenerV2Client { get; }

        /// <summary>
        /// Gets the client for managing event participant secrets in V2 API.
        /// </summary>
        IEventParticipantSecretV2Client EventParticipantSecretV2Client { get; }

        /// <summary>
        /// Gets the client for managing event participants in V2 API.
        /// </summary>
        IEventParticipantV2Client EventParticipantV2Client { get; }

        /// <summary>
        /// Gets the client for managing events in V2 API.
        /// </summary>
        IEventV2Client EventV2Client { get; }

        /// <summary>
        /// Gets the container exposing the V2 health check sub-clients.
        /// </summary>
        IHealthClientV2 HealthClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving archived listener events in V2 API.
        /// </summary>
        IListenerEventArchiveV2Client ListenerEventArchiveV2Client { get; }

        /// <summary>
        /// Gets the client for managing listener events in V2 API.
        /// </summary>
        IListenerEventV2Client ListenerEventV2Client { get; }

        /// <summary>
        /// Gets the client for replaying archived events in V2 API.
        /// </summary>
        IReplayingEventV2Client ReplayingEventV2Client { get; }
    }
}
