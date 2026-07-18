// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal interface IArchivingEventV2OrchestrationService
    {
        ValueTask<IEnumerable<EventV2>> RetrieveBatchOfQuarantinedEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<EventV2>> RetrieveBatchOfDeadEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default);
    }
}
