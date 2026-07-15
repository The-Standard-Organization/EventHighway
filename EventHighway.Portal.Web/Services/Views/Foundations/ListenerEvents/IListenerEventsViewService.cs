// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents;

namespace EventHighway.Portal.Web.Services.Views.Foundations.ListenerEvents
{
    public interface IListenerEventsViewService
    {
        ValueTask<List<ListenerEventView>> RetrieveAllListenerEventsAsync(
            CancellationToken cancellationToken = default);

        ValueTask<List<ListenerEventView>> RetrieveListenerEventsByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventView?> RetrieveListenerEventByIdAsync(
            Guid listenerEventId,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventView> RemoveListenerEventByIdAsync(
            Guid listenerEventId,
            CancellationToken cancellationToken = default);

        ValueTask<int> PurgeListenerEventsOlderThanAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default);
    }
}
