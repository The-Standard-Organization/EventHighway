// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<EventListenerV2> RegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventListenerV2Client
                    .RegisterEventListenerV2Async(eventListenerV2, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventListenerV2>>
            RetrieveEventListenerV2sByEventAddressIdAsync(
                Guid eventAddressV2Id,
                EventListenerV2Query eventListenerV2Query,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventListenerV2Client
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventAddressV2Id, eventListenerV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventListenerV2Client
                    .RemoveEventListenerV2ByIdAsync(eventListenerV2Id, cancellationToken),
                cancellationToken);
    }
}
