// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Foundations.Events.V2
{
    internal partial interface IEventV2Service
    {
        ValueTask<EventV2> AddEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default);
        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default);
        ValueTask<EventV2> ModifyEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default);
        ValueTask<EventV2> RemoveEventV2ByIdAsync(Guid eventV2Id, CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default);

        ValueTask<string> RemoveVolatilePathsAsync(EventV2 eventV2, CancellationToken cancellationToken = default);
    }
}
