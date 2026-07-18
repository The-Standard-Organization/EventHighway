// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default);
    }
}
