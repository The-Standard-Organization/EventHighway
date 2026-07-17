// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<IReadOnlyList<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            EventHandlerV2Query eventHandlerV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    eventHandlerV2Query, cancellationToken),
                cancellationToken);
    }
}
