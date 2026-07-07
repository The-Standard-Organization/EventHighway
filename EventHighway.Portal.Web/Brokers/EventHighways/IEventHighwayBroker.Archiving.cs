// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask ArchiveEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<List<EventArchiveV2Summary>> RetrieveAllEventArchiveV2SummariesAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventArchiveV2Summary?> RetrieveEventArchiveV2SummaryByIdAsync(
            Guid eventArchiveId,
            CancellationToken cancellationToken = default);
    }
}
