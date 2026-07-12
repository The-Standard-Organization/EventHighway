// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IEnumerable<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync<IEnumerable<EventHandlerV2>>(async client =>
                (await client.EventHandlerV2Client
                    .RetrieveAllEventHandlerV2sAsync(cancellationToken))
                    .ToList(),
                cancellationToken);
    }
}
