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
    public partial interface IEventHighwayBroker
    {
        ValueTask ArchiveEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sWithEventAddressV2Async(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default);
    }
}
