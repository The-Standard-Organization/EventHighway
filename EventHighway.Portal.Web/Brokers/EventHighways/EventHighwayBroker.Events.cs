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
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventV2Client.RetrieveAllEventV2sAsync(
                    eventV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventV2Client.RetrieveAllEventV2sWithEventAddressV2Async(
                    eventV2Query, cancellationToken),
                cancellationToken);
    }
}
