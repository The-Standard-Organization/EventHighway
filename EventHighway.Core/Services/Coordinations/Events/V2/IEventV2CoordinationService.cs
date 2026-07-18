// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Coordinations.Events.V2
{
    internal interface IEventV2CoordinationService
    {
        ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventV2>> RetrieveEventV2sByQueryAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventV2>> RetrieveEventV2sWithEventAddressV2ByQueryAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);

        ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);
    }
}
