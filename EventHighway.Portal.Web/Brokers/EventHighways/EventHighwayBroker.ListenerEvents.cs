// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync(
                    listenerEventV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            ListenerEventV2Query listenerEventV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.ListenerEventV2Client.RetrieveAllListenerEventV2sWithEventListenerV2Async(
                    listenerEventV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ListenerEventV2Client
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken),
                cancellationToken);
    }
}
