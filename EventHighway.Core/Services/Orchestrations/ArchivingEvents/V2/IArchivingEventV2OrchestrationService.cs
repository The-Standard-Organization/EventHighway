// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    public interface IArchivingEventV2OrchestrationService
    {
        ValueTask<IEnumerable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync();

        ValueTask<IEnumerable<EventV2>> RetrieveBatchOfDeadEventV2sAsync();

        ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfListenerEventV2sAsync(
            IEnumerable<Guid> eventV2Ids,
            CancellationToken cancellationToken = default);

        ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventV2AndListenerEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default);
    }
}
