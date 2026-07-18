// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.ListenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                    listenerEventArchiveV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.ListenerEventArchiveV2Client
                    .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                        listenerEventArchiveV2Query, cancellationToken),
                cancellationToken);
    }
}
