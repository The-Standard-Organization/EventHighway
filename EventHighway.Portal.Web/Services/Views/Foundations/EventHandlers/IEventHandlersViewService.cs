// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventHandlers
{
    public interface IEventHandlersViewService
    {
        ValueTask<List<EventHandlerView>> RetrieveAllEventHandlersAsync(
            CancellationToken cancellationToken = default);
    }
}
