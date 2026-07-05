// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal interface IArchivingEventV2CoordinationService
    {
        ValueTask ArchiveEventV2sAsync(CancellationToken cancellationToken = default);
        ValueTask PurgeEventArchiveV2sAsync(DateTimeOffset olderThan, CancellationToken cancellationToken = default);
        ValueTask PurgeEventArchiveV2sAsync(CancellationToken cancellationToken = default);
    }
}
