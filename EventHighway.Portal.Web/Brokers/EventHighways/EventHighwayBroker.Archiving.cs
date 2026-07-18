// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask ArchiveEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ArchivingEventV2Client.ArchiveEventV2sAsync(cancellationToken),
                cancellationToken);

        public ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ArchivingEventV2Client.PurgeEventArchiveV2sAsync(
                    olderThan, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventArchiveV2Client.RetrieveAllEventArchiveV2sAsync(
                    eventArchiveV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sWithEventAddressV2Async(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventArchiveV2Client.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    eventArchiveV2Query, cancellationToken),
                cancellationToken);
    }
}
