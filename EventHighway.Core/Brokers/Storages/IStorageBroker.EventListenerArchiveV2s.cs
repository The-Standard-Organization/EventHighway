// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventListenerArchiveV2> InsertEventListenerArchiveV2Async(
            EventListenerArchiveV2 eventListenerArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventListenerArchiveV2>> SelectAllEventListenerArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask InsertBulkEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask DeleteBulkEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
