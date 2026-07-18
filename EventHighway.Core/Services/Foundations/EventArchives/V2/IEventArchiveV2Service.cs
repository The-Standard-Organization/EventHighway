// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Core.Services.Foundations.EventArchives.V2
{
    internal interface IEventArchiveV2Service
    {
        ValueTask<EventArchiveV2> AddEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveEventArchiveV2sByQueryAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventArchiveV2> RemoveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<EventArchiveV2>> BulkAddEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
