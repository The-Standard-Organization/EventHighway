// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Processings.Events.V2
{
    internal interface IEventV2ProcessingService
    {
        ValueTask<EventV2> AddEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default);
        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync();
        ValueTask<IQueryable<EventV2>> RetrieveScheduledPendingEventV2sAsync();
        ValueTask<IQueryable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync();

        ValueTask<IEnumerable<EventV2>> RetrieveBatchOfDeadEventV2sAsync(int take);
        ValueTask<EventV2> MarkEventV2AsImmediateAsync(EventV2 eventV2, CancellationToken cancellationToken = default);
        ValueTask<EventV2> RemoveEventV2ByIdAsync(Guid eventV2Id, CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default);
    }
}
