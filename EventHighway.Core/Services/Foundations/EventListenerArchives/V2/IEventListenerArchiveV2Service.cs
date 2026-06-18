// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal interface IEventListenerArchiveV2Service
    {
        ValueTask<EventListenerArchiveV2> AddEventListenerArchiveV2Async(
            EventListenerArchiveV2 eventListenerArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventListenerArchiveV2>> RetrieveAllEventListenerArchiveV2sAsync();

        ValueTask<IEnumerable<EventListenerArchiveV2>> BulkAddEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventListenerArchiveV2sAsync(
            IEnumerable<EventListenerArchiveV2> eventListenerArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
