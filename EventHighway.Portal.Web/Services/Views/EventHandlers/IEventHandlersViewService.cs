// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.EventHandlers;

namespace EventHighway.Portal.Web.Services.Views.EventHandlers
{
    public interface IEventHandlersViewService
    {
        ValueTask<List<EventHandlerView>> RetrieveAllEventHandlersAsync(
            CancellationToken cancellationToken = default);
    }
}
