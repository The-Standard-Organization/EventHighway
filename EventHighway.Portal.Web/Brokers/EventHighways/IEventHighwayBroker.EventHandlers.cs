// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<IEnumerable<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default);
    }
}
