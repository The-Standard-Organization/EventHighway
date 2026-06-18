// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventArchiveV2> InsertEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventArchiveV2>> SelectAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventArchiveV2>>
            SelectAllEventArchiveV2sWithEventListenerArchiveV2sAndListenerEventArchiveV2sAsync(
                CancellationToken cancellationToken = default);

        ValueTask<EventArchiveV2> SelectEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventArchiveV2> DeleteEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask BulkInsertEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask BulkDeleteEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
