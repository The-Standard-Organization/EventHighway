// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventArchives
{
    public interface IEventArchivesViewService
    {
        ValueTask ArchiveProcessedEventsAsync(
            CancellationToken cancellationToken = default);

        ValueTask PurgeArchivesOlderThanAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default);

        ValueTask<List<EventArchiveView>> RetrieveAllEventArchivesAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventArchiveView> RetrieveEventArchiveByIdAsync(
            Guid eventArchiveId,
            CancellationToken cancellationToken = default);
    }
}
