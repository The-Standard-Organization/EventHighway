// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventListeners
{
    public interface IEventListenersViewService
    {
        ValueTask<List<EventListenerView>> RetrieveListenersByAddressAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerView> RegisterListenerAsync(
            EventListenerView listener,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerView> RemoveListenerByIdAsync(
            Guid listenerId,
            CancellationToken cancellationToken = default);
    }
}
