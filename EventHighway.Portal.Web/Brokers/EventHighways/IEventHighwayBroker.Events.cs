// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<List<EventV2Summary>> RetrieveAllEventV2SummariesAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2Summary?> RetrieveEventV2SummaryByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);
    }
}
